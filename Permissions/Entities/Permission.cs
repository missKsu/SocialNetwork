using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Permissions.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public Subject SubjectType { get; set; }
        public int SubjectId { get; set; }
        public Object ObjectType { get; set; }
        public int ObjectId { get; set; }
        public Operation Operation { get; set; }
    }

    public enum Operation
    {
        Write,
        Read,
        CantWrite,
        CantRead,
        Admin
    }

    public enum Subject
    {
        User
    }

    public enum Object
    {
        Group,
        Post,
        Permission
    }
}
