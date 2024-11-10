using SessionKeeper.Application;

namespace SessionKeeper.Cli;
public class CliCommandHandler : ICliCommandHandler
{
	private readonly ISessionManager _sessionManager;

	private bool _exit = false;

	public CliCommandHandler(ISessionManager sessionManager)
	{
		ArgumentNullException.ThrowIfNull(sessionManager);
		_sessionManager = sessionManager;
	}

	public void ListenConsole()
	{
		while(!_exit)
		{
			Console.Write("Введите команду: ");
			Handle(Console.ReadLine());
		}
	}

	private void Get(string sessionId)
	{
		var sessionResult = _sessionManager.GetSession(sessionId);
		if(sessionResult.IsFailed)
		{
			Console.WriteLine("Сессия отсутствует или истек срок ее жизни");
			return;
		}


		Console.WriteLine($"Сессия найдена: {sessionResult.Value.SessionId}");
	}

	private void Delete(string sessionId)
	{
		_sessionManager.DeleteSession(sessionId);
		Console.WriteLine("Сессия удалена");
	}

	private void Create(string login, string password)
	{
		var result = _sessionManager.AddSession(login, BCrypt.Net.BCrypt.HashPassword(password));

		Console.WriteLine(result.IsSuccess ? result.Value.SessionId.ToString() : result.Errors.Last().Message);
	}

	public bool Handle(string? command)
	{
		if(string.IsNullOrEmpty(command))
		{
			Console.WriteLine("Пустой ввод");
			return false;
		}
		var chunks = command.Split(' ');

		switch(chunks[0].ToLower())
		{
			case "get":
				if(chunks.Length != 2)
					return Error("Введено неправильное количество аргументов");
				Get(chunks[1]);
				return true;
			case "delete":
				if(chunks.Length != 2)
					return Error("Введено неправильное количество аргументов");
				Delete(chunks[1]);
				return true;
			case "create":
				if(chunks.Length != 3)
					return Error("Введено неправильное количество аргументов");
				Create(chunks[1], chunks[2]);
				return true;
			case "exit":
				_exit = true;
				return true;
			default:
				Console.WriteLine("Команда не распознана");
				return false;
		}
	}

	private bool Error(string message)
	{
		Console.WriteLine(message);
		return false;
	}
}
