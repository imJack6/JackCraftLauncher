using System.Reflection;
using System.Text;

namespace JackCraftLauncher.Class.Launch;

public class EncryptHandler
{
    public static string JcEncrypt(string rawStr)
    {
        var encryptStr = string.Empty;
        var encryptKey = Assembly.GetEntryAssembly()!.GetName().Name!;
        var encryptKeyLength = encryptKey.Length;
        var strLength = rawStr.Length;
        for (var i = 0; i < strLength; i++)
        {
            var k = i % encryptKeyLength;
            encryptStr += (char)(rawStr[i] ^ encryptKey[k]);
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptStr));
    }

    public static string JcDecrypt(string encryptStr)
    {
        try
        {
            var str = Encoding.UTF8.GetString(Convert.FromBase64String(encryptStr));
            var decryptStr = string.Empty;
            var encryptKey = Assembly.GetEntryAssembly()!.GetName().Name!;
            var encryptKeyLength = encryptKey.Length;
            var strLength = str.Length;
            for (var i = 0; i < strLength; i++)
            {
                var k = i % encryptKeyLength;
                decryptStr += (char)(str[i] ^ encryptKey[k]);
            }

            return decryptStr;
        }
        catch (Exception)
        {
            return encryptStr;
        }
    }
}