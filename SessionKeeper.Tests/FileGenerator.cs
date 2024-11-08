using System.Text.Json;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Tests;
internal static class FileGenerator
{
	public static FileInfo CreateUsersFile(int numberOfRecords)
	{
		var usersFile = new FileInfo($"users.{numberOfRecords}.json");
		var users = new User[numberOfRecords];

		for(int i = 0; i < numberOfRecords; i++)
		{
			users[i] = new User(Guid.NewGuid().ToString(), i.ToString());
		}

		using var writer = usersFile.OpenWrite();
		JsonSerializer.Serialize(writer, users);

		return usersFile;
	}

	public static FileInfo CreateSessionsFileForUsers(FileInfo usersFile, int numberOfRecords)
	{
		var sessionsFile = new FileInfo($"sessions.{numberOfRecords}.json");
		var sessions = new Session[numberOfRecords];

		return sessionsFile;
	}
}
