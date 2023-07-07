using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JackCraftLauncher.Class.ConfigHandler;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model.Microsoft.Graph;
using ProjBobcat.DefaultComponent.Authenticator;

namespace JackCraftLauncher.Class.Launch.AuthenticatorHandler;

public class AuthenticatorVerify
{
    // public int ExpiresIn = 0;
    // public DateTime LastRefreshedTime = DateTime.MinValue;
    public string XblRefreshToken = string.Empty;

    private static HttpClient DefaultClient => HttpClientHelper.DefaultClient;
    /*public AuthenticatorVerify()
    {
    }*/

    public async Task<(bool, GraphAuthResultModel?)> CacheTokenProviderAsync()
    {
        if (string.IsNullOrEmpty(XblRefreshToken)) return (false, default);
        /*if (string.IsNullOrEmpty(XblToken)) return (false, default);

        // 计算失效时间 
        var expireDate = LastRefreshedTime.AddSeconds(ExpiresIn);

        // 如果本地缓存令牌依旧是有效的，则直接返回当前令牌 
        // 否则，使用刷新令牌请求新的令牌 
        if (expireDate > DateTime.Now)
        {
            var result = new GraphAuthResultModel
            {
                ExpiresIn = (int)(expireDate - DateTime.Now).TotalSeconds,
                AccessToken = XblToken,
                RefreshToken = XblRefreshToken
            };

            return (true, result);
        }*/

        DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.MicrosoftLoginRefreshTokenNode,
            EncryptHandler.JcEncrypt(XblRefreshToken));
        // 请求新的登录令牌 
        var refreshReqDic = new List<KeyValuePair<string, string>>
        {
            new("client_id", MicrosoftAuthenticator.ApiSettings.ClientId),
            new("refresh_token", XblRefreshToken),
            new("grant_type", "refresh_token")
        };

        using var refreshReq = new HttpRequestMessage(HttpMethod.Post, MicrosoftAuthenticator.MSRefreshTokenRequestUrl)
        {
            Content = new FormUrlEncodedContent(refreshReqDic)
        };

        //using var refreshRes = await DefaultClient.SendAsync(refreshReq);
        var refreshRes = await DefaultClient.SendAsync(refreshReq);
        var refreshContent = await refreshRes.Content.ReadAsStringAsync();
        var refreshModel = MicrosoftAuthenticator.ResolveMSGraphResult(refreshContent,
            GraphAuthResultModelContext.Default.GraphAuthResultModel);

        if (refreshModel is not GraphAuthResultModel model)
        {
            if (refreshModel is GraphResponseErrorModel error)
            {
                // 在这里处理失败的刷新操作 
            }

            return (false, default);
        }

        DefaultConfigHandler.SetConfig(DefaultConfigConstants.LoginInformationNodes.MicrosoftLoginRefreshTokenNode,
            EncryptHandler.JcEncrypt(model.RefreshToken));

        return (true, model);
    }
}