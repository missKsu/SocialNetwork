using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posts.Entities
{
    public enum AuthorizeResult
    {
        Succeed,
        TokenExpired,
        WrongToken,
        NoToken
    }
}
