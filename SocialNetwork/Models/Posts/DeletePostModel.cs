using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Posts
{
    public class DeletePostModel
    {
        public int Id { get; set; }
        public string Group { get; set; }
        public string Creator { get; set; }
    }
}
