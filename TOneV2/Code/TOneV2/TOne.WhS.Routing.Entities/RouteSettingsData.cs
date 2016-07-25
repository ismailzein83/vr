﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteSettingsData : SettingData
    {
        public RouteDatabasesToKeep RouteDatabasesToKeep { get; set; }
    }

    public class RouteDatabasesToKeep
    {
        public RouteDatabaseConfiguration CustomerRouteConfiguration { get; set; }

        public RouteDatabaseConfiguration ProductRouteConfiguration { get; set; }
    }

    public class RouteDatabaseConfiguration
    {
        //public int SpecificDBToKeep { get; set; }

        public int CurrentDBToKeep { get; set; }

        public int FuturDBToKeep { get; set; }
    }
}
