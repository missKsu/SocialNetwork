using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Permissions
{
    public class AllPermissionsForUser
    {
        public List<PermissionModel> UserPermissions { get; set; }
        public List<PermissionModel> EditablePermissions { get; set; }
        public int SubjectId { get; set; }
    }
}
