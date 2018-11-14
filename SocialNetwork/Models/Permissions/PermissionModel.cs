using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Permissions
{
    public class PermissionModel
    {
        public string SubjectType { get; set; }
        public string SubjectName { get; set; }
        public string ObjectType { get; set; }
        public int ObjectId { get; set; }
        public string Operation { get; set; }
    }
}
