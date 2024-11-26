using System.Text.Json;

using Dapper;

using FluentResults;

using Npgsql;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Application.Repositories;
public class UserNpgsqlRepository(NpgsqlConnection connection) : IUserRepository
{
	private readonly string addUserSql = @"
		INSERT INTO public.""Users"" (Login, PasswordHash)
		VALUES (@Login, @PasswordHash)";

	private readonly string getUserByLoginSql = @"
		SELECT *
		FROM public.""Users""
		WHERE ""Login"" = @Login";

	public Result Authenticate(string login, string password)
	{
		var userRes = Get(login);

		if(userRes.IsFailed)
			return Result.Fail(userRes.Errors);

		var user = userRes.Value;

		if(!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
			return Result.Fail(new PasswordsDoNotMatchError());

		return Result.Ok();
	}

	public Result<User> Get(string login)
	{
		connection.Open();
		var user = connection.QueryFirstOrDefault<User>(getUserByLoginSql, new { Login = login });
		connection.Close();

		if(user == null)
			return Result.Fail(new UserDoesNotExistError());

		return user;
	}

	public void AddUsers(FileInfo initUsersFile)
	{
		if(!initUsersFile.Exists)
			throw new ArgumentException("Файл не существует");

		if(initUsersFile.Extension != ".json")
			throw new ArgumentException("Файл должен быть формата \".json\"");

		List<UserInfo> users;

		using(var reader = initUsersFile.OpenRead())
		{
			users = JsonSerializer.Deserialize<List<UserInfo>>(reader) ?? [];
		}

		connection.Open();

		connection.Execute(addUserSql, users);

		connection.Close();
	}
}
