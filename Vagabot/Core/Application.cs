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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Support;
using Binboo.Core.Configuration;
using Binboo.Core.Events;
using Binboo.Core.Persistence;
using SKYPE4COMLib;

namespace Binboo.Core
{
	public partial class Application : IDisposable
	{
		public event EventHandler Quit;
		public event ErrorEventHandler Error;
		public event EventHandler Attached;

		public Application(string prefix)
		{
			_prefix = prefix;
			_commandQueue = new CommandQueue(_exitEvent);
			
			_storageManager = new StorageManager(ConfigServices.StoragePath);

			StartCommandProcessor();
		}

		public IEnumerable Commands
		{
			get { return _commands.Values; }
		}

		public Application AddCommand(IBotCommand command)
		{
			command.Storage = _storageManager.StorageFor(command.Id);
			command.Initialize();

			_commands[NormalizeCommandName(command.Id)] = command;
			return this;
		}

		public void Stop()
		{
			UnregisterEvents();
			_exitEvent.Set();
		}

		private void UnregisterEvents()
		{
			_skype.MessageStatus -= ProcessMessage;
			((_ISkypeEvents_Event)_skype).AttachmentStatus -= ProcessAttachmentStatus;
		}

		public void AttachToSkype()
		{
			_ISkypeEvents_Event events = _skype;
			
			events.MessageStatus += ProcessMessage;
			events.AttachmentStatus += ProcessAttachmentStatus;
			events.CallStatus += CallStatusChanged;

			try
			{
				_skype.Attach(5, true);
			}
			catch(COMException ce)
			{
				RaiseErrorEvent("Unable to attach to skype.", ce);
			}
		}

		private void CallStatusChanged(Call pcall, TCallStatus status)
		{
			//if (status == TCallStatus.clsInProgress)
			//{
			//    pcall.set_OutputDevice(TCallIoDeviceType.callIoDeviceTypeFile, "c:\\temp\\output.wav");
			//    pcall.set_CaptureMicDevice(TCallIoDeviceType.callIoDeviceTypeFile, "c:\\temp\\output-mic.wav");
			//}
		}

		[Conditional("DEBUG")]
		private void DumpCall(string status, ICall pCall)
		{
			ICall x;

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
			}
			else if (Closing(status))
			{
				RaiseQuitEvent();
			}
		}

		private bool Closing(TAttachmentStatus status)
		{
			return status == TAttachmentStatus.apiAttachNotAvailable && _attached;
		}

		private void ProcessMessage(IChatMessage message, TChatMessageStatus status)
		{
			if (IsVagabotCommand(message) && IsComplete(status))
			{
				EnqueueCommand(message);
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

		private bool IsVagabotCommand(IChatMessage message)
		{
			return message.Body.StartsWith("$" + _prefix, StringComparison.CurrentCultureIgnoreCase);
		}

		private void StartCommandProcessor()
		{
			Thread processorThread = new Thread(CommandQueueProcessor) { IsBackground = true };
			processorThread.Start(_commandQueue);
		}

		private void CommandQueueProcessor(object obj)
		{
			var queue = (CommandQueue) obj;
			while(WaitHandle.WaitAny(new [] {_exitEvent}, 1, true) == WaitHandle.WaitTimeout)
			{
				IChatMessage command = queue.Next();
				if (IsSafeToProcess(command))
				{
					ProcessCommands(command);
				}
				else
				{
					//TODO: Log
				}
			}
		}

		private static bool IsSafeToProcess(IChatMessage command)
		{
			return command != null && !command.Sender.IsBlocked;
		}

		private void ProcessCommands(IChatMessage message)
		{
			try
			{
				SafeProcessCommands(message);
			}
			catch(Exception ex)
			{
				RaiseErrorEvent("Error processing command.", ex);
			}
		}

		private void SafeProcessCommands(IChatMessage message)
		{
			var results = new List<string>();
			ICommandResult result = CommandResult.None;
			foreach (var commandLine in CommandsFor(message.Body))
			{
				result = ProcessCommand(message.Sender.Handle, commandLine, result);
				results.Add(result.HumanReadable);

				if (result.Status != CommandStatus.Success) break;
			}

			message.Chat.SendMessage(string.Join(Environment.NewLine, results.ToArray()));
		}

		private ICommandResult ProcessCommand(string user, string commandLine, ICommandResult previousResullt)
		{
			try
			{
				return SafeProcessCommand(user, commandLine, previousResullt);
			}
			catch(Exception ex)
			{
				return CommandResult.Exception(ex);
			}
		}

		private ICommandResult SafeProcessCommand(string user, string commandLine, ICommandResult previousResullt)
		{
			var ctx = PipedContextFor(user, commandLine, previousResullt);
			return RunCommand(ctx, commandLine);
		}

		internal static IEnumerable<string> CommandsFor(string message)
		{
			message = StripBotName(message);

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

		private static string StripBotName(string message)
		{
			return Regex.Replace(message, @"\$[A-Za-z]+\s+", "");
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

		private ICommandResult RunCommand(IContext ctx, string commandLine)
		{
			IBotCommand command = GetCommand(commandLine);
			return command.Process(ctx);
		}

		private static IContext PipedContextFor(string user, string commandLine, ICommandResult result)
		{
			var passedArguments = CommandParameters(commandLine);
			return result.PipeThrough(passedArguments, pipedArgs => new Context(user, pipedArgs));
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

		private IBotCommand GetCommand(string message)
		{
			string commandName = GetNormilizedCommandName(message);
			return _commands.ContainsKey(commandName) ? _commands[commandName] : new UnknowCommand(commandName, _commands);
		}

		private static string GetCommandName(string message)
		{
			string[] strings = Regex.Split(message, " ");
			return strings[0];
		}

		private static string GetNormilizedCommandName(string message)
		{
			return NormalizeCommandName(GetCommandName(message));
		}

		private static string NormalizeCommandName(string id)
		{
			return id.ToLower().Trim();
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

		public void Dispose()
		{
			_storageManager.Dispose();
		}
		
		private readonly string _prefix;
		private readonly IDictionary<string, IBotCommand> _commands = new Dictionary<string, IBotCommand>();
		private readonly CommandQueue _commandQueue;
		private IStorageManager _storageManager;

		private Skype _skype = new Skype();
		private bool _attached;
		private readonly EventWaitHandle _exitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
	}
}