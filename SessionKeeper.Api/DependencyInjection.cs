using SessionKeeper.Application;
using SessionKeeper.Application.Repositories;

namespace SessionKeeper.Api;

public static class DependencyInjection
{
	public static IServiceCollection AddDependencies(this IServiceCollection services, string initUsersFilePath, string usersFilePath, string sessionsFilePath)
	{
		var initUsersFile = new FileInfo(initUsersFilePath);
		var usersFile = new FileInfo(usersFilePath);
		usersFile.Create().Close();
		var sessionsFile = new FileInfo(sessionsFilePath);

		services
			.AddSingleton<IUserRepository>(new UserJsonRepository(initUsersFile, usersFile))
			.AddSingleton<ISessionRepository>(new SessionJsonRepository(sessionsFile))
			.AddSingleton<ISessionManager, SessionManager>();

		return services;
	}
}
