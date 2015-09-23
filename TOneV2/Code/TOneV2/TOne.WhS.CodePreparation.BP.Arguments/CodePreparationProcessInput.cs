﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.BP.Arguments
{
    public class CodePreparationProcessInput : BaseProcessInputArgument
    {
        public int SaleZonePackageId { get; set; }
        public int FileId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public override string GetTitle()
        {
            return String.Format("CodePreparation Process Started for Package: {0}", SaleZonePackageId);
        }


    }
}
