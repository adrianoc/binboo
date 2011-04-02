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
using System.Threading;
using SKYPE4COMLib;

namespace Binboo.Tests.Mocks
{
	public class ChatMock : Chat
	{
		private readonly IList<ChatMessage> _messages = new List<ChatMessage>();
		private readonly EventWaitHandle _messageEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

		private readonly IUser _user;

		public ChatMock(IUser user)
		{
			_user = user;
		}

		public ChatMessage SendMessage(string messageText)
		{
			ChatMessage msg = new ChatMessageMock(_user, messageText, this);

			_messages.Add(msg);
			_messageEvent.Set();

			return msg;
		}

		public IEnumerable<ChatMessage> SentMessages
		{
			get
			{
				return _messages;
			}
		}

		public bool WaitForMessages(int timeout)
		{
			bool ret = _messageEvent.WaitOne(timeout);
			_messageEvent.Reset();

			return ret;
		}

		public void Reset()
		{
			_messages.Clear();
		}

		public void OpenWindow()
		{
			throw new System.NotImplementedException();
		}

		public void Leave()
		{
			throw new System.NotImplementedException();
		}

		public void AddMembers(UserCollection pMembers)
		{
			throw new System.NotImplementedException();
		}

		public void Bookmark()
		{
			throw new System.NotImplementedException();
		}

		public void Unbookmark()
		{
			throw new System.NotImplementedException();
		}

		public void SetPassword(string Password, string Hint)
		{
			throw new System.NotImplementedException();
		}

		public void Join()
		{
			throw new System.NotImplementedException();
		}

		public void Kick(string Handle)
		{
			throw new System.NotImplementedException();
		}

		public void KickBan(string Handle)
		{
			throw new System.NotImplementedException();
		}

		public void Disband()
		{
			throw new System.NotImplementedException();
		}

		public void EnterPassword(string Password)
		{
			throw new System.NotImplementedException();
		}

		public void ClearRecentMessages()
		{
			throw new System.NotImplementedException();
		}

		public void AcceptAdd()
		{
			throw new System.NotImplementedException();
		}

		public string Name
		{
			get { throw new System.NotImplementedException(); }
		}

		public ChatMessageCollection Messages
		{
			get { throw new System.NotImplementedException(); }
		}

		public DateTime Timestamp
		{
			get { throw new System.NotImplementedException(); }
		}

		public User Adder
		{
			get { throw new System.NotImplementedException(); }
		}

		public TChatStatus Status
		{
			get { throw new System.NotImplementedException(); }
		}

		public UserCollection Posters
		{
			get { throw new System.NotImplementedException(); }
		}

		public UserCollection Members
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Topic
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public UserCollection ActiveMembers
		{
			get { throw new System.NotImplementedException(); }
		}

		public string FriendlyName
		{
			get { throw new System.NotImplementedException(); }
		}

		public ChatMessageCollection RecentMessages
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool Bookmarked
		{
			get { throw new System.NotImplementedException(); }
		}

		public string TopicXML
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public IChatMemberCollection MemberObjects
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Blob
		{
			get { throw new System.NotImplementedException(); }
		}

		public int Options
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public string PasswordHint
		{
			get { throw new System.NotImplementedException(); }
		}

		public string GuideLines
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public string Description
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public string DialogPartner
		{
			get { throw new System.NotImplementedException(); }
		}

		public DateTime ActivityTimestamp
		{
			get { throw new System.NotImplementedException(); }
		}

		public TChatMemberRole MyRole
		{
			get { throw new System.NotImplementedException(); }
		}

		public UserCollection Applicants
		{
			get { throw new System.NotImplementedException(); }
		}

		public string AlertString
		{
			set { throw new System.NotImplementedException(); }
		}

		public TChatType Type
		{
			get { throw new System.NotImplementedException(); }
		}

		public TChatMyStatus MyStatus
		{
			get { throw new System.NotImplementedException(); }
		}
	}
}