using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Groups
{
    public class EditGroupModel
    {
        public string Name { get; set; }
        public int Creator { get; set; }
        public string Description { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
        public string User { get; set; }
    }
}
