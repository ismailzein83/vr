﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class StudentQuery
    {
        public string Name { get; set; }
        public List<int> RoomIds { get; set; }
        public List<int> BuildingIds { get; set; }
    }
}
