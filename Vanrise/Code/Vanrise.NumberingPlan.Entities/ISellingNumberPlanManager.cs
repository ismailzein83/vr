﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public interface ISellingNumberPlanManager : IBEManager
    {
        String GetSellingNumberPlanName(int sellingNumberPlanId);

    }
}
