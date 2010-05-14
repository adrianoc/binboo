using Moq;
using SKYPE4COMLib;

namespace Binboo.Tests
{
	public abstract class TestWithUser
	{
		protected User NewUserMock()
		{
			var userMock = new Mock<User>();
			userMock.Setup(user => user.IsBlocked).Returns(false);
			userMock.Setup(user => user.Handle).Returns("test");
			return userMock.Object;
		}

	}
}
