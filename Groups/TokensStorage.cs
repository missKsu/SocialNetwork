using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Groups
{
    public class TokensStorage
    {
        private List<string> tokens = new List<string>();

        public void AddToken(string token)
        {
            tokens.Add(token);
        }

        public bool CheckToken(string token)
        {
            return tokens.Contains(token);
        }
    }
}
