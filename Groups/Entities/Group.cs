﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Groups.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Creator { get; set; }
        //public string Description { get; set; }
    }
}
