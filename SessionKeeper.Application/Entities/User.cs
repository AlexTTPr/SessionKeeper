using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionKeeper.Application.Entities;
public class User(string login, string passwordHash) /*: IParsable<LoginInfo>*/
{
    public string Login { get; private set; } = login;
    public string PasswordHash { get; private set; } = passwordHash;

    //public override string ToString()
    //{
    //	return $"{Login} {Password}";
    //}

    //#region Parse
    //public static LoginInfo Parse(string s, IFormatProvider? provider)
    //{
    //	if(!TryParse(s, provider, out var info))
    //		throw new ArgumentException("Ошибка при попытке чтения данных логина");

    //	return info;
    //}

    //public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out LoginInfo result)
    //{
    //	result = null;

    //	if(s is null)
    //		return false;

    //	var loginInfoParams = s.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

    //	if(loginInfoParams.Length != 2)
    //		return false;

    //	result = new LoginInfo(loginInfoParams[0], loginInfoParams[1]);
    //	return true;
    //}
    //#endregion
}
