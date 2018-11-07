using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posts.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public int Author { get; set; }
        public string Text { get; set; }
        public int Group { get; set; }
    }
}
