using Moq;
using SKYPE4COMLib;

namespace Binboo.Tests
{
	public abstract class TestWithUser
	{
		protected IUser NewUserMock()
		{
			var userMock = new Mock<IUser>();
			userMock.Setup(user => user.IsBlocked).Returns(false);
			userMock.Setup(user => user.Handle).Returns("test");
			return userMock.Object;
		}

	}
}
