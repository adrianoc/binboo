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
using Moq;
using SKYPE4COMLib;

namespace Binboo.Tests.Mocks
{
	public class SkypeMock : Skype
	{
		private readonly Func<Chat> _chatRetriever;

		public  SkypeMock(Func<Chat> chatRetriever)
		{
			_chatRetriever = chatRetriever;	
		}

		public UserCollection SearchForUsers(string Target)
		{
			throw new System.NotImplementedException();
		}

		public void Attach(int Protocol, bool Wait)
		{
		}

		public Call PlaceCall(string Target, string Target2, string Target3, string Target4)
		{
			throw new System.NotImplementedException();
		}

		public ChatMessage SendMessage(string username, string text)
		{
			var message = new ChatMessageMock(NewUserMock(username), text, _chatRetriever());
			if (null != MessageStatus)
			{
				MessageStatus(message, TChatMessageStatus.cmsReceived);
			}

			return message;
		}

		private static User NewUserMock(string username)
		{
			var userMock = new Mock<User>();
			userMock.Setup(user => user.IsBlocked).Returns(false);
			userMock.Setup(user => user.Handle).Returns(username);

			return userMock.Object;
		}

		public void SendCommand(Command pCommand)
		{
			throw new System.NotImplementedException();
		}

		public void ChangeUserStatus(TUserStatus newVal)
		{
			throw new System.NotImplementedException();
		}

		public Chat CreateChatWith(string Username)
		{
			throw new System.NotImplementedException();
		}

		public Chat CreateChatMultiple(UserCollection pMembers)
		{
			throw new System.NotImplementedException();
		}

		public Voicemail SendVoicemail(string Username)
		{
			throw new System.NotImplementedException();
		}

		public void ClearChatHistory()
		{
			throw new System.NotImplementedException();
		}

		public void ClearVoicemailHistory()
		{
			throw new System.NotImplementedException();
		}

		public void ClearCallHistory(string Username, TCallHistory Type)
		{
			throw new System.NotImplementedException();
		}

		public void ResetCache()
		{
			throw new System.NotImplementedException();
		}

		public Group CreateGroup(string GroupName)
		{
			throw new System.NotImplementedException();
		}

		public void DeleteGroup(int GroupId)
		{
			throw new System.NotImplementedException();
		}

		public void EnableApiSecurityContext(TApiSecurityContext Context)
		{
			throw new System.NotImplementedException();
		}

		public SmsMessage CreateSms(TSmsMessageType MessageType, string TargetNumbers)
		{
			throw new System.NotImplementedException();
		}

		public SmsMessage SendSms(string TargetNumbers, string MessageText, string ReplyToNumber)
		{
			throw new System.NotImplementedException();
		}

		public int AsyncSearchUsers(string Target)
		{
			throw new System.NotImplementedException();
		}

		public Chat FindChatUsingBlob(string Blob)
		{
			throw new System.NotImplementedException();
		}

		public Chat CreateChatUsingBlob(string Blob)
		{
			throw new System.NotImplementedException();
		}

		public int Timeout
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public string CurrentUserHandle
		{
			get { throw new System.NotImplementedException(); }
		}

		public TUserStatus CurrentUserStatus
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		TConnectionStatus ISkype.ConnectionStatus
		{
			get { throw new System.NotImplementedException(); }
		}

		bool ISkype.Mute
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public string Version
		{
			get { throw new System.NotImplementedException(); }
		}

		public User CurrentUser
		{
			get { throw new System.NotImplementedException(); }
		}

		public Conversion Convert
		{
			get { throw new System.NotImplementedException(); }
		}

		public UserCollection Friends
		{
			get { throw new System.NotImplementedException(); }
		}

		public CallCollection ActiveCalls
		{
			get { throw new System.NotImplementedException(); }
		}

		public CallCollection MissedCalls
		{
			get { throw new System.NotImplementedException(); }
		}

		public ChatMessageCollection MissedMessages
		{
			get { throw new System.NotImplementedException(); }
		}

		TAttachmentStatus ISkype.AttachmentStatus
		{
			get { throw new System.NotImplementedException(); }
		}

		public int Protocol
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public ChatCollection Chats
		{
			get { throw new System.NotImplementedException(); }
		}

		public ConferenceCollection Conferences
		{
			get { throw new System.NotImplementedException(); }
		}

		public ChatCollection ActiveChats
		{
			get { throw new System.NotImplementedException(); }
		}

		public ChatCollection MissedChats
		{
			get { throw new NotImplementedException(); }
		}

		public ChatCollection RecentChats
		{
			get { throw new System.NotImplementedException(); }
		}

		public ChatCollection BookmarkedChats
		{
			get { throw new System.NotImplementedException(); }
		}

		public VoicemailCollection Voicemails
		{
			get { throw new System.NotImplementedException(); }
		}

		public UserCollection UsersWaitingAuthorization
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool CommandId
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public bool Cache
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public Profile CurrentUserProfile
		{
			get { throw new System.NotImplementedException(); }
		}

		public GroupCollection Groups
		{
			get { throw new System.NotImplementedException(); }
		}

		public GroupCollection CustomGroups
		{
			get { throw new System.NotImplementedException(); }
		}

		public GroupCollection HardwiredGroups
		{
			get { throw new System.NotImplementedException(); }
		}

		public Settings Settings
		{
			get { throw new System.NotImplementedException(); }
		}

		public Client Client
		{
			get { throw new System.NotImplementedException(); }
		}

		public string FriendlyName
		{
			set { throw new System.NotImplementedException(); }
		}

		public VoicemailCollection MissedVoicemails
		{
			get { throw new System.NotImplementedException(); }
		}

		public SmsMessageCollection Smss
		{
			get { throw new System.NotImplementedException(); }
		}

		public SmsMessageCollection MissedSmss
		{
			get { throw new System.NotImplementedException(); }
		}

		public string ApiWrapperVersion
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool SilentMode
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public IFileTransferCollection FileTransfers
		{
			get { throw new System.NotImplementedException(); }
		}

		public IFileTransferCollection ActiveFileTransfers
		{
			get { throw new System.NotImplementedException(); }
		}

		public UserCollection FocusedContacts
		{
			get { throw new System.NotImplementedException(); }
		}

		public string PredictiveDialerCountry
		{
			get { throw new System.NotImplementedException(); }
		}

		public string get_Property(string ObjectType, string ObjectId, string PropName)
		{
			throw new System.NotImplementedException();
		}

		public void set_Property(string ObjectType, string ObjectId, string PropName, string pVal)
		{
			throw new System.NotImplementedException();
		}

		public string get_Variable(string Name)
		{
			throw new System.NotImplementedException();
		}

		public void set_Variable(string Name, string pVal)
		{
			throw new System.NotImplementedException();
		}

		public bool get_Privilege(string Name)
		{
			throw new System.NotImplementedException();
		}

		public CallCollection get_Calls(string Target)
		{
			throw new System.NotImplementedException();
		}

		public ChatMessageCollection get_Messages(string Target)
		{
			throw new System.NotImplementedException();
		}

		public User get_User(string Username)
		{
			throw new System.NotImplementedException();
		}

		public ChatMessage get_Message(int Id)
		{
			throw new System.NotImplementedException();
		}

		public Call get_Call(int Id)
		{
			throw new System.NotImplementedException();
		}

		public Chat get_Chat(string Name)
		{
			throw new System.NotImplementedException();
		}

		public Conference get_Conference(int Id)
		{
			throw new System.NotImplementedException();
		}

		public string get_Profile(string Property)
		{
			throw new System.NotImplementedException();
		}

		public void set_Profile(string Property, string pVal)
		{
			throw new System.NotImplementedException();
		}

		public SKYPE4COMLib.Application get_Application(string Name)
		{
			throw new System.NotImplementedException();
		}

		public Voicemail get_Greeting(string Username)
		{
			throw new System.NotImplementedException();
		}

		public Command get_Command(int Id, string Command, string Reply, bool Block, int Timeout)
		{
			throw new System.NotImplementedException();
		}

		public Voicemail get_Voicemail(int Id)
		{
			throw new System.NotImplementedException();
		}

		public bool get_ApiSecurityContextEnabled(TApiSecurityContext Context)
		{
			throw new System.NotImplementedException();
		}

		public event _ISkypeEvents_CommandEventHandler Command;
		public event _ISkypeEvents_ReplyEventHandler Reply;
		public event _ISkypeEvents_ErrorEventHandler Error;
		event _ISkypeEvents_AttachmentStatusEventHandler _ISkypeEvents_Event.AttachmentStatus
		{
			add { }
			remove { }
		}

		event _ISkypeEvents_ConnectionStatusEventHandler _ISkypeEvents_Event.ConnectionStatus
		{
			add { throw new System.NotImplementedException(); }
			remove { throw new System.NotImplementedException(); }
		}

		public event _ISkypeEvents_UserStatusEventHandler UserStatus;
		public event _ISkypeEvents_OnlineStatusEventHandler OnlineStatus;
		public event _ISkypeEvents_CallStatusEventHandler CallStatus;
		public event _ISkypeEvents_CallHistoryEventHandler CallHistory;
		event _ISkypeEvents_MuteEventHandler _ISkypeEvents_Event.Mute
		{
			add { throw new System.NotImplementedException(); }
			remove { throw new System.NotImplementedException(); }
		}

		public event _ISkypeEvents_MessageStatusEventHandler MessageStatus;
		public event _ISkypeEvents_MessageHistoryEventHandler MessageHistory;
		public event _ISkypeEvents_AutoAwayEventHandler AutoAway;
		public event _ISkypeEvents_CallDtmfReceivedEventHandler CallDtmfReceived;
		public event _ISkypeEvents_VoicemailStatusEventHandler VoicemailStatus;
		public event _ISkypeEvents_ApplicationConnectingEventHandler ApplicationConnecting;
		public event _ISkypeEvents_ApplicationStreamsEventHandler ApplicationStreams;
		public event _ISkypeEvents_ApplicationDatagramEventHandler ApplicationDatagram;
		public event _ISkypeEvents_ApplicationSendingEventHandler ApplicationSending;
		public event _ISkypeEvents_ApplicationReceivingEventHandler ApplicationReceiving;
		public event _ISkypeEvents_ContactsFocusedEventHandler ContactsFocused;
		public event _ISkypeEvents_GroupVisibleEventHandler GroupVisible;
		public event _ISkypeEvents_GroupExpandedEventHandler GroupExpanded;
		public event _ISkypeEvents_GroupUsersEventHandler GroupUsers;
		public event _ISkypeEvents_GroupDeletedEventHandler GroupDeleted;
		public event _ISkypeEvents_UserMoodEventHandler UserMood;
		public event _ISkypeEvents_SmsMessageStatusChangedEventHandler SmsMessageStatusChanged;
		public event _ISkypeEvents_SmsTargetStatusChangedEventHandler SmsTargetStatusChanged;
		public event _ISkypeEvents_CallInputStatusChangedEventHandler CallInputStatusChanged;
		public event _ISkypeEvents_AsyncSearchUsersFinishedEventHandler AsyncSearchUsersFinished;
		public event _ISkypeEvents_CallSeenStatusChangedEventHandler CallSeenStatusChanged;
		public event _ISkypeEvents_PluginEventClickedEventHandler PluginEventClicked;
		public event _ISkypeEvents_PluginMenuItemClickedEventHandler PluginMenuItemClicked;
		public event _ISkypeEvents_WallpaperChangedEventHandler WallpaperChanged;
		public event _ISkypeEvents_FileTransferStatusChangedEventHandler FileTransferStatusChanged;
		public event _ISkypeEvents_CallTransferStatusChangedEventHandler CallTransferStatusChanged;
		public event _ISkypeEvents_ChatMembersChangedEventHandler ChatMembersChanged;
		public event _ISkypeEvents_ChatMemberRoleChangedEventHandler ChatMemberRoleChanged;
		public event _ISkypeEvents_CallVideoStatusChangedEventHandler CallVideoStatusChanged;
		public event _ISkypeEvents_CallVideoSendStatusChangedEventHandler CallVideoSendStatusChanged;
		public event _ISkypeEvents_CallVideoReceiveStatusChangedEventHandler CallVideoReceiveStatusChanged;
		public event _ISkypeEvents_SilentModeStatusChangedEventHandler SilentModeStatusChanged;
		public event _ISkypeEvents_UILanguageChangedEventHandler UILanguageChanged;
		public event _ISkypeEvents_UserAuthorizationRequestReceivedEventHandler UserAuthorizationRequestReceived;
	}
}