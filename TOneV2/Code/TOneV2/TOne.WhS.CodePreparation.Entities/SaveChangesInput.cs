﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP;

namespace TOne.WhS.CodePreparation.Entities
{
    public class SaveChangesInput
    {
        public int SellingNumberPlanId { get; set; }
        public Changes NewChanges { get; set; }
    }
}
