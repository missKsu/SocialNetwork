using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Permissions.Entities
{
    public enum AuthorizeResult
    {
        Succeed,
        TokenExpired,
        WrongToken,
        NoToken
    }
}
