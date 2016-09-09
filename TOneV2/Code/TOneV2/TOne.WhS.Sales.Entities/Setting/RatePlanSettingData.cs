﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class RatePlanSettingsData : SettingData
    {
        public int IncreasedRateDayOffset { get; set; }
        public int DecreasedRateDayOffset { get; set; }
        public int NewServiceDayOffset { get; set; }
    }
}
