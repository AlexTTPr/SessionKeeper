using System.Text;

using Microsoft.Extensions.DependencyInjection;

using SessionKeeper.Application;
using SessionKeeper.Application.Repositories;

namespace SessionKeeper.Cli;

internal class Program
{
	public static void Main(string[] args)
	{
		Console.OutputEncoding = Encoding.UTF8;

		Console.WriteLine("Session Keeper");

		string filePath;
		do
		{
			Console.Write("Введите полный путь файла с данными аккаунтов: ");
			filePath = Console.ReadLine()!;
		} while(!File.Exists(filePath));

		var initUsersFile = new FileInfo(filePath);

		var usersFile = new FileInfo("users.json");
		usersFile.Create().Close();

		var sessionsFile = new FileInfo("sessions.json");

		var serviceProvider = new ServiceCollection()
			.AddSingleton<IUserRepository>(new UserJsonRepository(initUsersFile, usersFile))
			.AddSingleton<ISessionRepository>(new SessionJsonRepository(sessionsFile))
			.AddSingleton<ISessionManager, SessionManager>()
			.AddSingleton<ICliCommandHandler, CliCommandHandler>()
			.BuildServiceProvider();

		var commandHandler = serviceProvider.GetService<ICliCommandHandler>();

		commandHandler!.ListenConsole();
	}
}
