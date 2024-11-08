using FluentResults;

using Moq;

using SessionKeeper.Application;
using SessionKeeper.Application.Entities;
using SessionKeeper.Application.Repositories;

namespace SessionKeeper.Tests;
[TestFixture]
internal class SessionManagerTests
{
	private SessionManager _sessionManager;
	private Mock<IUserRepository> _userRepositoryMock;
	private Mock<ISessionRepository> _sessionRepositoryMock;

	[SetUp]
	public void SetUp()
	{
		_userRepositoryMock = new Mock<IUserRepository>();
		_sessionRepositoryMock = new Mock<ISessionRepository>();

		_sessionManager = new SessionManager(_userRepositoryMock.Object, _sessionRepositoryMock.Object);
	}

	[Test]
	public void AddSession_Fails_When_UserDoesNotExist()
	{
		_userRepositoryMock
			.Setup(e => e.Authenticate("1", "1"))
				.Returns(Result.Fail(new UserDoesNotExistError()));

		Assert.That(_sessionManager.AddSession("1", "1").Errors.Single() is UserDoesNotExistError);
	}

	[Test]
	public void AddSession_Success_When_UserExist()
	{

		_userRepositoryMock
			.Setup(e => e.Authenticate("1", "1"))
			.Returns(Result.Ok());

		_sessionRepositoryMock
			.Setup(e => e.Create(It.IsAny<Session>()))
			.Returns(Result.Ok());


		Assert.That(_sessionManager.AddSession("1", "1").IsSuccess);
	}
}
