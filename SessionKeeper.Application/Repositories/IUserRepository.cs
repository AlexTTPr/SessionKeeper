using FluentResults;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Application.Repositories;
public interface IUserRepository
{
	Result<User> Get(string login);
	Result Authenticate(string login, string passwordHash);
}
