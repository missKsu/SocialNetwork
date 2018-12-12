using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Models.Posts
{
    public class AllPostsModel
    {
        public List<PostModel> Posts { get; set; }
        public int page { get; set; }
    }
}
