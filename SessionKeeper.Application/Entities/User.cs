namespace SessionKeeper.Application.Entities;
public class User
{
	protected User() { }

	public User(string login, string passwordHash)
	{
		Login = login;
		PasswordHash = passwordHash;
	}

	public string Login { get; private set; }
	public string PasswordHash { get; private set; }
}
