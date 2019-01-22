using IdentityModel.Client;
using SocialNetwork.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork
{
    public class TokensStorage
    {
        private Dictionary<string,(DateTime expiry, string refreshToken)> dict = new Dictionary<string, (DateTime expiry, string refreshToken)>();
        //private List<Tuple<string, int>> tokens = new List<Tuple<string, int>>();

        public void AddToken(string token, string refreshToken)
        {
            if (token is null)
                return;
            dict.Add(token, (DateTime.Now + TimeSpan.FromSeconds(600), refreshToken));
            //tokens.Add(new Tuple<string, int>(token, date));
        }

        public AuthorizeResult CheckToken(string token)
        {
            if (token == "")
                return AuthorizeResult.NoToken;
            var client = new IntrospectionClient("http://localhost:5005/connect/introspect", "api", "api-secret");
            var resp = client.SendAsync(new IntrospectionRequest { Token = token }).Result;
            if (resp.IsActive)
                return AuthorizeResult.Succeed;
            var result = dict.ContainsKey(token);
            if (!result)
                return AuthorizeResult.WrongToken;
            var resultToken = dict[token];
            if (resultToken.expiry < DateTime.Now)
            {
                if (resultToken.expiry + TimeSpan.FromSeconds(3600) > DateTime.Now)
                    return AuthorizeResult.TokenExpired;
                else
                    return AuthorizeResult.WrongToken;
            }
            return AuthorizeResult.Succeed;
        }

        public string RefreshToken(string token)
        {
            var refreshToken = dict[token];
            return refreshToken.refreshToken;
            //dict.Add(token, (DateTime.Now + TimeSpan.FromSeconds(600), refreshToken));
            //tokens.Add(new Tuple<string, int>(token, date));
        }
    }
}
