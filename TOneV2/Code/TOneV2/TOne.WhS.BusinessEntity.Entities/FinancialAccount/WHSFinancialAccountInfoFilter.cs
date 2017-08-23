﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountInfoFilter
    {
        public Guid? FinancialAccountDefinitionId { get; set; }
        public VRAccountStatus? Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public WHSFinancialAccountCarrierType? CarrierType { get; set; }
        public Guid? BalanceAccountTypeId { get; set; }
        public Guid? InvoiceTypeId { get; set; }

    }
}
