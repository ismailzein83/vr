using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.MainExtensions.RetailBillingCharge
{
    public class RetailBillingAnalyticQueryCharge : Retail.Billing.Entities.RetailBillingCharge
    {
        /// <summary>
        /// needs to be of type Generic RecordFilterGroup
        /// </summary>
        public string FilterGroup { get; set; }
    }
}