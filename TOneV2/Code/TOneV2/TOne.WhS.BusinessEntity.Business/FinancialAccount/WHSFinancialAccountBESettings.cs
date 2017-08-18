﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class WHSFinancialAccountDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public Guid? BalanceAccountTypeId { get; set; }

        public Guid? InvoiceTypeId { get; set; }

        public WHSFinancialAccountDefinitionExtendedSettings ExtendedSettings { get; set; }

        #region BusinessEntityDefinitionSettings

        public static Guid S_CONFIGID = new Guid("9C9B6E19-B05C-426D-88CF-9B25509CF4C5");
        public override Guid ConfigId
        {
            get { return S_CONFIGID; }
        }
        public override string IdType
        {
            get { return "System.Int32"; }
        }

        public override string ManagerFQTN
        {
            get { return "TOne.WhS.BusinessEntity.Business.WHSFinancialAccountManager, TOne.WhS.BusinessEntity.Business"; }
        }

        public override string SelectorUIControl
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
