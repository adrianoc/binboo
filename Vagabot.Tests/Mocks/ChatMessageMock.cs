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
using SKYPE4COMLib;

namespace Binboo.Tests.Mocks
{
	internal class ChatMessageMock : ChatMessage
	{
		private readonly string _body;
		private readonly IUser _sender;
		private readonly Chat _chat;

		public ChatMessageMock(IUser sender, string body, IChat chat)
		{
			_body = body;
			_sender = sender;
			_chat = (Chat) chat;
		}

		public int Id
		{
			get { throw new NotImplementedException(); }
		}

		public DateTime Timestamp
		{
			get { throw new NotImplementedException(); }
		}

		public string FromHandle
		{
			get { return _sender.DisplayName; }
		}

		public string FromDisplayName
		{
			get { throw new System.NotImplementedException(); }
		}

		public TChatMessageType Type
		{
			get { throw new System.NotImplementedException(); }
		}

		public TChatMessageStatus Status
		{
			get { throw new System.NotImplementedException(); }
		}

		public TChatLeaveReason LeaveReason
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Body
		{
			get { return _body; }
			set { throw new System.NotImplementedException(); }
		}

		public string ChatName
		{
			get { throw new System.NotImplementedException(); }
		}

		public UserCollection Users
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool Seen
		{
			set { throw new System.NotImplementedException(); }
		}

		public Chat Chat
		{
			get
			{
				return _chat;
			}
		}

		public User Sender
		{
			get { return (User) _sender ;}
		}

		public string EditedBy
		{
			get { throw new System.NotImplementedException(); }
		}

		public DateTime EditedTimestamp
		{
			get { throw new System.NotImplementedException(); }
		}

		public TChatMemberRole Role
		{
			get { throw new System.NotImplementedException(); }
		}

		public int Options
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsEditable
		{
			get { throw new System.NotImplementedException(); }
		}
	}
}