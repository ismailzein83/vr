﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Arguments
{
    public class SaveDailyStatisticsToDBProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int SwitchID { get; set; }

        public override string GetTitle()
        {
            return String.Format("Save Daily Statistics To DB Process for Switch {0}", SwitchID);
        }
    }
}
