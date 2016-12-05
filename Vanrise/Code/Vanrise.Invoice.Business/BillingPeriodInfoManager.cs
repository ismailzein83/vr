using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class BillingPeriodInfoManager
    {
        public BillingPeriodInfo GetBillingPeriodInfoById(string partnerId , Guid invoiceTypeId)
        {
            IBillingPeriodInfoDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IBillingPeriodInfoDataManager>();
            return dataManager.GetBillingPeriodInfoById(partnerId , invoiceTypeId);
        }
        public bool InsertOrUpdateBillingPeriodInfo(BillingPeriodInfo billingPeriodInfo)
        {
            IBillingPeriodInfoDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IBillingPeriodInfoDataManager>();
            return dataManager.InsertOrUpdateBillingPeriodInfo(billingPeriodInfo);
        }
    }
}
