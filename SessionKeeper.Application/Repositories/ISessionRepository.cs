using FluentResults;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Application.Repositories;
public interface ISessionRepository
{
	Result<Session> Get(string sessionId);
	Result<Session> Create(Session session);
	Result Delete(string sessionId);
}