using System.Collections.Frozen;
using System.Text.Json;

using FluentResults;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Application.Repositories;
public class UserJsonRepository : IUserRepository
{
	private readonly FileInfo _usersFile;
	private readonly IDictionary<string, User> _users;

	public UserJsonRepository(FileInfo initUsersFile, FileInfo usersFile)
	{
		if(!initUsersFile.Exists)
			throw new ArgumentException("Файл не существует");

		if(initUsersFile.Extension != ".json")
			throw new ArgumentException("Файл должен быть формата \".json\"");

		if(!usersFile.Exists)
			throw new ArgumentException("Файл не существует");

		if(usersFile.Extension != ".json")
			throw new ArgumentException("Файл должен быть формата \".json\"");

		_usersFile = usersFile;

		File.WriteAllText(_usersFile.FullName, string.Empty);

		List<UserInfo> users;

		using(var reader = initUsersFile.OpenRead())
		{
			users = JsonSerializer.Deserialize<List<UserInfo>>(reader) ?? [];
		}

		_users = users.ToFrozenDictionary(
			e => e.Login,
			e => new User(e.Login, BCrypt.Net.BCrypt.HashPassword(e.Password)));

		using var writer = _usersFile.OpenWrite();
		JsonSerializer.Serialize(writer, users);
	}

	public Result<User> Get(string login)
	{
		if(!_users.TryGetValue(login, out var result))
			return new UserDoesNotExistError();

		return result;
	}

	public Result Authenticate(string login, string password)
	{
		if(!_users.TryGetValue(login, out var user))
			return new UserDoesNotExistError();

		if(!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
			return new PasswordsDoNotMatchError();

		return Result.Ok();
	}
}

public class UserInfo
{
	public string Login { get; set; }
	public string Password { get; set; }
}

public class UserDoesNotExistError : Error
{
	public UserDoesNotExistError() : base("Пользователя не существует") { }
}

public class PasswordsDoNotMatchError : Error
{
	public PasswordsDoNotMatchError() : base("Пароли не совпадают") { }
}