using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Permissions
{
    public class AllPermissionsModel
    {
        public List<PermissionModel> Permissions { get; set; }
        public int SubjectId { get; set; }
    }
}
