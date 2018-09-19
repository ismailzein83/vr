﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace NP.IVSwitch.Entities
{
    public enum RouteTableViewType
    {
        ANumber = 0,
        Whitelist = 1,
        BNumber=2
    }
    public class RouteTableViewSettings:ViewSettings
    {
        public override string GetURL(View view)
        {
            return String.Format("#/viewwithparams/NP_IVSwitch/Views/RouteTable/RouteTableManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
        public List<RouteTableViewType> Types { get; set; }
     
    }
}
