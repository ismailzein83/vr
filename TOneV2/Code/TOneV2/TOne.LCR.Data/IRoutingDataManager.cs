﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Data
{
    public interface IRoutingDataManager
    {
        int DatabaseId { set; }
        RoutingDatabaseType RoutingDatabaseType { set;  }
    }
}
