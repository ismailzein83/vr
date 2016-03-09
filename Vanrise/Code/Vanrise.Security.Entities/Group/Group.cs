﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class Group
    {
        public int GroupId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public GroupSettings Settings { get; set; }
    }
}
