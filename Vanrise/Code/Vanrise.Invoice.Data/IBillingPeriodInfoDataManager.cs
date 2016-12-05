using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public  interface IBillingPeriodInfoDataManager:IDataManager
    {
        BillingPeriodInfo GetBillingPeriodInfoById(string partnerId, Guid invoiceTypeId);
        bool InsertOrUpdateBillingPeriodInfo(BillingPeriodInfo billingPeriodInfo);
    }
}
