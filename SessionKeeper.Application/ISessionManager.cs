using FluentResults;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Application;
public interface ISessionManager
{
	Result<Session> AddSession(string login, string passwordHash);
	Result DeleteSession(string sessionId);
	Result<Session> GetSession(string sessionId);
}
