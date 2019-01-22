using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Identity
{
    public enum AuthorizeResult
    {
        Succeed,
        TokenExpired,
        WrongToken,
        NoToken
    }
}
