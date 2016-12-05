using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Invoice.Data.SQL
{
    public class BillingPeriodInfoDataManager:BaseSQLDataManager,IBillingPeriodInfoDataManager
    {
        #region ctor
        public BillingPeriodInfoDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }
        #endregion
        public Entities.BillingPeriodInfo GetBillingPeriodInfoById(string partnerId, Guid invoiceTypeId)
        {
            return GetItemSP("VR_Invoice.sp_BillingPeriodInfo_Get", BillingPeriodInfoMapper, invoiceTypeId, partnerId);
        }
        public bool InsertOrUpdateBillingPeriodInfo(Entities.BillingPeriodInfo billingPeriodInfo)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_BillingPeriodInfo_InsertOrUpdate", billingPeriodInfo.InvoiceTypeId, billingPeriodInfo.PartnerId, billingPeriodInfo.NextPeriodStart);
            return (affectedRows > -1);
        }
        #region Mappers
        public Entities.BillingPeriodInfo BillingPeriodInfoMapper(IDataReader reader)
        {
            Entities.BillingPeriodInfo invoice = new Entities.BillingPeriodInfo
            {
                NextPeriodStart = GetReaderValue<DateTime>(reader, "NextPeriodStart"),
                InvoiceTypeId = GetReaderValue<Guid>(reader, "InvoiceTypeId"),
                PartnerId = reader["PartnerId"] as string,
            };
            return invoice;
        }
        #endregion


        
    }

           
}
