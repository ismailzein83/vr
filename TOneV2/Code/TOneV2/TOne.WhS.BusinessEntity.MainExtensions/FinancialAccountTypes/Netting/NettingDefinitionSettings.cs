﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.Netting
{
    public class NettingDefinitionSettings : WHSFinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsApplicableToCustomer
        {
            get { return true; }
        }

        public override bool IsApplicableToSupplier
        {
            get { return true; }
        }
    }
}
