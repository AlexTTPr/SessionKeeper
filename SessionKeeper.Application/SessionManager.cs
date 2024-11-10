using FluentResults;

using SessionKeeper.Application.Entities;
using SessionKeeper.Application.Repositories;


namespace SessionKeeper.Application;
public class SessionManager : ISessionManager
{
	private readonly IUserRepository _userRepository;
	private readonly ISessionRepository _sessionRepository;

	public SessionManager(IUserRepository userRepository, ISessionRepository sessionRepository)
	{
		_userRepository = userRepository;
		_sessionRepository = sessionRepository;
	}

	public Result<Session> AddSession(string login, string passwordHash)
	{
		var authenticatedResult = _userRepository.Authenticate(login, passwordHash);

		if(authenticatedResult.IsFailed)
			return Result.Fail(authenticatedResult.Errors);

		var session = Session.Create(login);

		return _sessionRepository.Create(session);
	}

	public Result DeleteSession(string sessionId)
	{
		return _sessionRepository.Delete(sessionId);
	}

	public Result<Session> GetSession(string sessionId)
	{
		return _sessionRepository.Get(sessionId);
	}
}
