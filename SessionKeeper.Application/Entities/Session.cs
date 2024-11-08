namespace SessionKeeper.Application.Entities;
public class Session(Guid sessionId, string owner, DateTime created, TimeSpan lifeTime)/* : IParsable<Session>*/
{
	public Guid SessionId { get; private set; } = sessionId;
	public string Owner { get; private set; } = owner;
	public DateTime Created { get; private set; } = created;
	public TimeSpan LifeTime { get; private set; } = lifeTime;

	//NOTE: Фабричный метод
	public static Session Create(string owner, DateTime? created = null, TimeSpan? lifeTime = null)
	{
		return new Session(Guid.NewGuid(), owner, created ?? DateTime.UtcNow, lifeTime ?? TimeSpan.FromMinutes(15));
	}

	public bool IsAlive() => Created + LifeTime >= DateTime.UtcNow;

	//public override string ToString()
	//{
	//	return $"{SessionId} {Owner} {Created} {LifeTime}";
	//}

	//#region Parse
	//public static Session Parse(string s, IFormatProvider? provider)
	//{
	//	if(!TryParse(s, provider, out var session))
	//		throw new ArgumentException("Ошибка при попытке чтения данных сессии");

	//	return session;
	//}

	//public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Session result)
	//{
	//	result = null;

	//	if(s is null)
	//		return false;

	//	var SessionParams = s.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

	//	if(SessionParams.Length != 4)
	//		return false;

	//	if(!Guid.TryParse(SessionParams[0], out var guid))
	//		return false;

	//	if(!DateTime.TryParse(SessionParams[2], out DateTime created))
	//		return false;

	//	if(!TimeSpan.TryParse(SessionParams[3], out TimeSpan lifeTime))
	//		return false;

	//	result = new Session(guid, SessionParams[1], created, lifeTime);
	//	return true;
	//}
	//#endregion
}
