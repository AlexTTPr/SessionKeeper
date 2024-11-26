using Microsoft.Extensions.Caching.Distributed;

using Npgsql;

using SessionKeeper.Application;
using SessionKeeper.Application.Repositories;

namespace SessionKeeper.Api;

public static class DependencyInjection
{
	public static IServiceCollection UseJsonStrorage(this IServiceCollection services, string initUsersFilePath, string usersFilePath, string sessionsFilePath)
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

	public static IServiceCollection AddNpgStorage(this IServiceCollection services, string npgConnectionString)
	{
		services.AddSingleton(new NpgsqlConnection(npgConnectionString));
		return services;
	}

	public static IServiceCollection AddRedisCache(this IServiceCollection services, string redisConnectionString)
	{
		services.AddStackExchangeRedisCache(cfg =>
		{
			cfg.Configuration = redisConnectionString;
			cfg.InstanceName = "SessionKeeper";
		});
		var redisOptions = new DistributedCacheEntryOptions()
		{
			SlidingExpiration = TimeSpan.FromMinutes(15)
		};
		services.AddSingleton(redisOptions);

		return services;
	}


	public static IServiceCollection UseNpgsqlAndRedisStorages(this IServiceCollection services)
	{

		services
			.AddSingleton<IUserRepository, UserNpgsqlRepository>()
			.AddSingleton<ISessionRepository, CachedSessionRepository>()
			.AddSingleton<ISessionManager, SessionManager>();
		return services;
	}
}
