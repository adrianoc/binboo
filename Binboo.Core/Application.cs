/**
 * Copyright (c) 2009 Adriano Carlos Verona
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 **/
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Binboo.Core.Commands;
using Binboo.Core.Exceptions;
using Binboo.Core.Persistence;
using Binboo.Core.Plugins;
using log4net;
using log4net.Config;
using SKYPE4COMLib;
using ErrorEventArgs = Binboo.Core.Events.ErrorEventArgs;
using ErrorEventHandler = Binboo.Core.Events.ErrorEventHandler;
using IUser = Binboo.Core.Framework.IUser;

namespace Binboo.Core
{
	public partial class Application : IDisposable
	{
		public event EventHandler Quit;
		public event ErrorEventHandler Error;
		public event EventHandler Attached;

		static Application()
		{
			XmlConfigurator.Configure(new FileInfo("Binboo.config.xml"));
		}

        public static Application WithPluginsFrom(ComposablePartCatalog catalog)
        {
			try
			{
				return new Application(catalog);
			}
			catch(ReflectionTypeLoadException rtle)
			{
				GetLogger().ErrorFormat("Could not start application, exiting. Error: {0}", rtle.LoaderExceptions);
				throw;
			}
			catch(Exception ex)
			{
				GetLogger().ErrorFormat("Could not start application, exiting. Error: {0}", ex);
				throw;
			}
        }

        public void Dispose()
        {
			//TODO: Notify plugins that we are going to unload.
			//BUG: Dispose() is not being invoked anymore....
            var storageManager = StorageManager;
            if (storageManager != null)
            {
                storageManager.Dispose();
            }
        }

	    public ISet<IPlugin> Plugins
	    {
            get { return _plugins; }
	    }

		public void Stop()
		{
			UnregisterEvents();
			_exitEvent.Set();
		}

		private void UnregisterEvents()
		{
			var skypeEvents = ((_ISkypeEvents_Event)_skype);

			skypeEvents.MessageStatus -= ProcessMessage;
			skypeEvents.AttachmentStatus -= ProcessAttachmentStatus;
			skypeEvents.Reply -= ProcessReply;
		}

		public void AttachToSkype()
		{
			var events = (_ISkypeEvents_Event) _skype;

			events.MessageStatus += ProcessMessage;
			events.AttachmentStatus += ProcessAttachmentStatus;
			events.Reply += ProcessReply;

			try
			{
				_skype.Attach(8);
			}
			catch(COMException ce)
			{
				RaiseErrorEvent("Unable to attach to skype.", ce);
			}
		}

		private void ProcessReply(Command command)
		{
			try
			{
				var match = Regex.Match(command.Reply, @"CHATMESSAGE (?<MSG_ID>\d+) EDITED_TIMESTAMP (?<EDITED_TIMESTAMP>\d+)");
				if (match.Success)
				{
					var msg = _skype.Message[int.Parse(match.Groups["MSG_ID"].Value)];
					ProcessMessage(msg, TChatMessageStatus.cmsSent);
				}
			}
			catch (Exception ex)
			{
				RaiseErrorEvent("Error in Reply event handler.", ex);
			}
		}

		[Conditional("DEBUG")]
		private void DumpCall(string status, ICall pCall)
		{
			Debug.WriteLine(
				String.Format(
					"----------------------\r\nCall: {0}\r\n"+
					"Conference: {1}\r\n"+
					"Status: {2}\r\n" +
					"PartnerDisplayName: {3} ({4})\r\n" +
					"Subject: {5}\r\n" +
					"Type: {6}\r\n" +
					"Failure Reason: {7}\r\n" +
					"+++++++++++++++++++++++\r\n",
					pCall.Id, 
					pCall.ConferenceId,
					status, 
					pCall.PartnerDisplayName,
					pCall.PartnerHandle,
					pCall.Subject,
					pCall.Type,
					pCall.FailureReason));
			
			if (pCall.Participants != null)
			{
				foreach (IParticipant participant in pCall.Participants)
				{
					Debug.WriteLine("Participant: " + participant.DisplayName);
				}
			}

			if (pCall.ConferenceId != 0)
			{
				foreach (IConference conference in _skype.Conferences)
				{
					if (conference.Id == pCall.ConferenceId)
					{
						foreach (ICall call in conference.ActiveCalls)
						{
							Debug.WriteLine("Participant: " + call.PartnerDisplayName);
						}
					}
				}
			}
		}

		private void ProcessAttachmentStatus(TAttachmentStatus status)
		{
			if (status == TAttachmentStatus.apiAttachSuccess)
			{
				_attached = true;
				RaiseAttachedEvent();
				ProcessMissedMessages();
			}
			else if (Closing(status))
			{
				RaiseQuitEvent();
			}
		}

		private void ProcessMissedMessages()
		{
			ChatMessageCollection missedMessages = _skype.MissedMessages;
			_log.InfoFormat("Processing {0} messages received while offline'", missedMessages.Count);
			for (int i = 1; i <= missedMessages.Count; i++)
			{
				ProcessMessage(missedMessages[i], TChatMessageStatus.cmsReceived);
			}
		}

		private bool Closing(TAttachmentStatus status)
		{
			return status == TAttachmentStatus.apiAttachNotAvailable && _attached;
		}

		private void ProcessMessage(IChatMessage message, TChatMessageStatus status)
		{
			_log.DebugFormat("Got message ({0}): '{1}'", message.FromHandle, message.Body);
			if (IsBotCommand(message) && IsComplete(status))
			{
				EnqueueCommand(message);
				_log.DebugFormat("Message enqueued ({0}): '{1}'", message.FromHandle, message.Body);
			}
		}

		private static bool IsComplete(TChatMessageStatus status)
		{
			return (status == TChatMessageStatus.cmsReceived || status == TChatMessageStatus.cmsSent);
		}

		private void EnqueueCommand(IChatMessage command)
		{
			_commandQueue.Add(command);
		}

		private bool IsBotCommand(IChatMessage message)
		{
            if (!message.Body.IsPluginCommand()) return false;
            var pluginName = message.Body.PluginName();
		    return _plugins.Any(p => StringComparer.CurrentCultureIgnoreCase.Compare(p.Name, pluginName) == 0); 
		}

		private void StartCommandProcessor()
		{
			_log.InfoFormat("Starting command processor.");
			var processorThread = new Thread(CommandQueueProcessor) { IsBackground = true };
			processorThread.Start(_commandQueue);
		}

		private void CommandQueueProcessor(object obj)
		{
			SetThreadName();
			
			var queue = (CommandQueue) obj;
			_log.Info("Command processor thread started.");

			while(WaitHandle.WaitAny(new [] {_exitEvent}, 1, true) == WaitHandle.WaitTimeout)
			{
				IChatMessage command = queue.Next();
				ProcessCommands(command);
			}

			_log.Info("Command processor thread finished.");
		}

		private static void SetThreadName()
		{
			Thread.CurrentThread.Name = "CommandProcessor";
		}

		private static bool IsSafeToProcess(IChatMessage command)
		{
			return command != null && !command.Sender.IsBlocked;
		}

		private void ProcessCommands(IChatMessage message)
		{
			if (message == null) return;
			
			_log.InfoFormat("Received command: {0}", message.Body);
			if (!IsSafeToProcess(message))
			{
				_log.InfoFormat("Command ignored (not safe): {0}", message.Body);
				return;
			}

			try
			{
				SafeProcessCommands(message);
			}
			catch(Exception ex)
			{
				_log.Error("Error processing command.", ex);
				RaiseErrorEvent("Error processing command.", ex);
			}
		}

		private void SafeProcessCommands(IChatMessage message)
		{
		    IPlugin plugin = _plugins.Where( p => p.Name == message.Body.PluginName() ).Single();
            var results = new List<string>();
			ICommandResult result = CommandResult.None;
			foreach (var commandLine in CommandsFor(message.Body))
			{
				_log.DebugFormat("Processing command line: {0}", commandLine);

                result = ProcessCommand(message.Sender.AsUser(), plugin, commandLine, result);
				results.Add(result.HumanReadable);

				_log.Info(result);
				if (result.Status != CommandStatus.Success) break;
			}

			var resultMessage = string.Join(Environment.NewLine, results.ToArray());
			_log.DebugFormat("Sending result message: '{0}'", resultMessage);

			message.Chat.SendMessage(resultMessage);
		}

	    private static ICommandResult ProcessCommand(IUser user, IPlugin plugin, string commandLine, ICommandResult previousResullt)
		{
			try
			{
                return SafeProcessCommand(user, plugin, commandLine, previousResullt);
			}
			catch(Exception ex)
			{
				return CommandResult.Exception(ex);
			}
		}

        private static ICommandResult SafeProcessCommand(IUser user, IPlugin plugin, string commandLine, ICommandResult previousResullt)
		{
			var ctx = PipedContextFor(plugin, user, commandLine, previousResullt);
			return plugin.ExecuteCommand(GetCommandName(commandLine), ctx);
		}

		public static IEnumerable<string> CommandsFor(string message)
		{
			message = message.StripPluginName();

			int startPos = 0;
			int currentPos;

			for (int pos = message.IndexOf('|'); pos != -1; pos = message.IndexOf('|', currentPos) )
			{
				if (!InsideQuotedText(message, pos, startPos))
				{
					yield return message.Substring(startPos, pos - startPos - 1).Trim();
					startPos = pos + 1;
				}

				currentPos = pos + 1;
			}

			yield return message.Substring(startPos).Trim();
		}

		private static bool InsideQuotedText(string message, int pos, int startPos)
		{
			return CharCount(message, startPos, pos) % 2 == 1;
		}

		private static int CharCount(string buffer, int startPos, int endPos)
		{
			var count = 0;
			for (int i = startPos; i < endPos && i < buffer.Length; i++)
			{
				if (buffer[i] == '"') count++;
			}

			return count;
		}

	    private static IContext PipedContextFor(IPlugin plugin, IUser user, string commandLine, ICommandResult result)
		{
			var passedArguments = CommandParameters(commandLine);
			return result.PipeThrough(passedArguments, pipedArgs => new Context(user, plugin, pipedArgs));
		}

		private static string CommandParameters(string commandLine)
		{
			MatchCollection arguments = Regex.Matches(
									commandLine,
									@"[A-Za-z][A-Za-z0-9]*\s(?<param>.*)", 
									RegexOptions.IgnoreCase | RegexOptions.Multiline |
									RegexOptions.CultureInvariant |
									RegexOptions.IgnorePatternWhitespace |
									RegexOptions.Compiled);

			return arguments.Count == 1 
				? arguments[0].Groups["param"].Value 
				: string.Empty;
		}

		private static string GetCommandName(string message)
		{
			string[] strings = Regex.Split(message, " ");
			return strings[0];
		}
        
		private void RaiseErrorEvent(string message, Exception e)
		{
			ErrorEventHandler errorHandler = Error;
			if (errorHandler != null)
			{
				errorHandler(this, new ErrorEventArgs(message, e.ToString()));
			}

			Stop();
		}

		private void RaiseQuitEvent()
		{
			RaiseEvent(Quit, EventArgs.Empty);
		}

		private void RaiseAttachedEvent()
		{
			RaiseEvent(Attached, EventArgs.Empty);
		}

		private void RaiseEvent(EventHandler handler, EventArgs e)
		{
			if (handler != null)
			{
				handler(this, e);
			}
		}

        private Application(ComposablePartCatalog catalog)
        {
            _commandQueue = new CommandQueue(_exitEvent);
		
        	InitializePlugins(catalog);

			StartCommandProcessor();
        }

		private void InitializePlugins(ComposablePartCatalog catalog)
		{
			var pluginManager = PluginManagerFactory.Create(catalog);
			_plugins = pluginManager.Plugins;

			foreach (var plugin in Plugins)
			{
				_log.InfoFormat("Initializing plugin '{0}'.", plugin.Name);
				try
				{
					plugin.Initialize();
					_log.InfoFormat("'{0}' initialized successfuly", plugin.Name);
				}
				catch(Exception ex)
				{
					_log.ErrorFormat("Plugin '{0}' failed to initialize and will be disabled. See next entry for more details.", plugin.Name);
					_log.Error(ex);

					plugin.Enabled = false;
				}
			}
		}

		private static ILog GetLogger()
		{
			return LogManager.GetLogger(typeof(Application));
		}

		[Import]
        private IStorageManager StorageManager { get; set; }

	    private ISet<IPlugin> _plugins = new HashSet<IPlugin>();
		private readonly CommandQueue _commandQueue;

        private readonly ILog _log = GetLogger();

		private ISkype _skype = new Skype();
		private bool _attached;
		private readonly EventWaitHandle _exitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
	}
}
