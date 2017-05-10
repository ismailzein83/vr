﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class CurrentMonthTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId
        {
            get { return new Guid("D03B778A-47DA-474D-8A20-87F76C75145A"); }
        }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            context.FromTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
            context.ToTime = DateTime.Now;
        }
    }
}
