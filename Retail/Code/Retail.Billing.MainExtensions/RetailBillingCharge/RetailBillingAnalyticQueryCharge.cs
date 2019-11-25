using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.RetailBillingCharge
{
    public class RetailBillingAnalyticQueryCharge : Retail.Billing.Entities.RetailBillingCharge
    {
        /// <summary>
        /// needs to be of type Generic RecordFilterGroup
        /// </summary>
        public RecordFilterGroup FilterGroup { get; set; }
        public override string GetDescription()
        {
            return "Retail Billing Analytic Query Charge";
        }
    }
}