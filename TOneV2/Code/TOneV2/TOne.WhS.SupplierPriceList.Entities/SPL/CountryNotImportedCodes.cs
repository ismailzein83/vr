﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class CountryNotImportedCodes : Vanrise.BusinessProcess.Entities.IRuleTarget
    {
        public int CountryId { get; set; }
        public List<NotImportedCode> NotImportedCodes { get; set; }
        public object Key
        {
            get { return default(object); }
        }
        public string TargetType
        {
            get { return "Pricelist"; }
        }
    }
}
