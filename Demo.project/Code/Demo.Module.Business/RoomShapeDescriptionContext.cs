﻿using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
   public class RoomShapeDescriptionContext :IRoomShapeDescriptionContext
   {
       public Room Room { get; set; }
    }
}
