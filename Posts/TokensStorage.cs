using Posts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posts
{
    public class TokensStorage
    {
        private List<Tuple<string, int>> tokens = new List<Tuple<string, int>>();

        public void AddToken(string token)
        {
            var date = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
            tokens.Add(new Tuple<string, int>(token, date));
        }

        public AuthorizeResult CheckToken(string token)
        {
            if (token == "")
                return AuthorizeResult.NoToken;
            var result = tokens.LastOrDefault(t => t.Item1 == token);
            if (result == null)
                return AuthorizeResult.WrongToken;
            var date = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
            if (result.Item2 + 30 > date)
                return AuthorizeResult.TokenExpired;
            return AuthorizeResult.Succeed;
        }
    }
}
