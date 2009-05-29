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
	public class UserMock : User
	{
		private bool _isBlocked;
		private string _handle;

		public UserMock(string handle, bool isBlocked)
		{
			_handle = handle;
			_isBlocked = isBlocked;
		}

		public string Handle
		{
			get { return _handle; }
			set { throw new System.NotImplementedException(); }
		}

		public string FullName
		{
			get { throw new System.NotImplementedException(); }
		}

		public DateTime Birthday
		{
			get { throw new System.NotImplementedException(); }
		}

		public TUserSex Sex
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Country
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Province
		{
			get { throw new System.NotImplementedException(); }
		}

		public string City
		{
			get { throw new System.NotImplementedException(); }
		}

		public string PhoneHome
		{
			get { throw new System.NotImplementedException(); }
		}

		public string PhoneOffice
		{
			get { throw new System.NotImplementedException(); }
		}

		public string PhoneMobile
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Homepage
		{
			get { throw new System.NotImplementedException(); }
		}

		public string About
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool HasCallEquipment
		{
			get { throw new System.NotImplementedException(); }
		}

		public TBuddyStatus BuddyStatus
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public bool IsAuthorized
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public bool IsBlocked
		{
			get { return _isBlocked; }
			set { throw new System.NotImplementedException(); }
		}

		public string DisplayName
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public TOnlineStatus OnlineStatus
		{
			get { throw new System.NotImplementedException(); }
		}

		public DateTime LastOnline
		{
			get { throw new System.NotImplementedException(); }
		}

		public string CountryCode
		{
			get { throw new System.NotImplementedException(); }
		}

		public string ReceivedAuthRequest
		{
			get { throw new System.NotImplementedException(); }
		}

		public string SpeedDial
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public bool CanLeaveVoicemail
		{
			get { throw new System.NotImplementedException(); }
		}

		public string MoodText
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Aliases
		{
			get { throw new System.NotImplementedException(); }
		}

		public int Timezone
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsCallForwardActive
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Language
		{
			get { throw new System.NotImplementedException(); }
		}

		public string LanguageCode
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsVideoCapable
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsSkypeOutContact
		{
			get { throw new System.NotImplementedException(); }
		}

		public int NumberOfAuthBuddies
		{
			get { throw new System.NotImplementedException(); }
		}

		public string RichMoodText
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsVoicemailCapable
		{
			get { throw new System.NotImplementedException(); }
		}
	}
}