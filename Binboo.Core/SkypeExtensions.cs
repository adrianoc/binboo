using Binboo.Core.Framework;

namespace Binboo.Core
{
    public static class SkypeExtensions
    {
        public static IUser AsUser(this SKYPE4COMLib.User skypeUser)
        {
            return new User(skypeUser.Handle, string.Empty, skypeUser.LanguageCode ?? skypeUser.CountryCode);
        }
    }
}
