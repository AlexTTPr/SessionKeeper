using System.Text.Json;

using FluentResults;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Application.Repositories;
public class SessionJsonRepository : ISessionRepository
{
	private readonly FileInfo _sessionsFile;

	private IDictionary<string, Session> _sessions;

	public SessionJsonRepository(FileInfo sessionFile)
	{
		if(!sessionFile.Exists)
			sessionFile.Create().Close();

		if(sessionFile.Extension != ".json")
			throw new ArgumentException("Файл должен быть формата \".json\"");

		_sessionsFile = sessionFile;

		using var sessionReader = _sessionsFile.OpenRead();
		List<Session> deserializedSessions;
		try
		{
			deserializedSessions = JsonSerializer.Deserialize<List<Session>>(sessionReader) ?? [];
		}
		catch
		{
			deserializedSessions = [];
		}
		_sessions = deserializedSessions.ToDictionary(e => e.SessionId.ToString());
	}


	public Result<Session> Create(Session session)
	{
		_sessions[session.SessionId.ToString()] = session;

		UpdateSessions();
		return session;
	}

	public Result Delete(string sessionId)
	{
		if(!_sessions.Remove(sessionId))
			return new SessionDoesNotExistError();

		UpdateSessions();
		return Result.Ok();
	}

	public Result<Session> Get(string sessionId)
	{
		if(!_sessions.TryGetValue(sessionId, out var session))
			return new SessionDoesNotExistError();

		if(!session.IsAlive())
			return new SessionIsNotAliveError();

		return session!;
	}

	private void UpdateSessions()
	{
		File.Delete(_sessionsFile.FullName);
		using var writer = _sessionsFile.OpenWrite();
		var sessions = _sessions.Values;
		JsonSerializer.Serialize(writer, sessions);
	}

	public Result ClearDeadSessions()
	{
		_sessions = _sessions
			.Where(kv => kv.Value.IsAlive())
			.ToDictionary(kv => kv.Key, kv => kv.Value);

		UpdateSessions();
		return Result.Ok();
	}
}

public class SessionDoesNotExistError : Error
{
	public SessionDoesNotExistError() : base("Сессия не существует") { }
}

public class SessionIsNotAliveError : Error
{
	public SessionIsNotAliveError() : base("У сессии закончился срок жизни") { }
}