using System.Text.Json;

using FluentResults;

using Microsoft.Extensions.Caching.Distributed;

using SessionKeeper.Application.Entities;

namespace SessionKeeper.Application.Repositories;
public class CachedSessionRepository(IDistributedCache cache, DistributedCacheEntryOptions _options) : ISessionRepository
{
	public Result<Session> Create(Session session)
	{
		cache.SetStringAsync(session.SessionId.ToString(), JsonSerializer.Serialize(session), _options);

		return Result.Ok();
	}

	public Result Delete(string sessionId)
	{
		cache.RemoveAsync(sessionId);

		return Result.Ok();
	}

	public Result<Session> Get(string sessionId)
	{
		var sessionString = cache.GetString(sessionId);
		if(sessionString == null)
			return Result.Fail(new SessionDoesNotExistError());

		var session = JsonSerializer.Deserialize<Session>(sessionString);

		return session;
	}
}
