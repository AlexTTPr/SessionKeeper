using FluentResults;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Application.Repositories;
public interface IUserRepository
{
	Result<User> Get(string sessionId);
	Result Authenticate(string login, string passwordHash);
}
