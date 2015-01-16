using System;
using System.Collections.Generic;

namespace TABS.BusinessEntities
{
    public class Billing_InvoiceBO
    {
        public static void DeleteBillingInvoice(int invoiceID)
        {
            string sqlString = string.Format(@"
                DELETE FROM Billing_Invoice_Details  WHERE InvoiceID = {0}
                DELETE FROM Billing_Invoice WHERE InvoiceID = {0}", invoiceID);
            TABS.DataHelper.ExecuteNonQuery(sqlString);
        }

        public static object Execute_SP_BP_ErroneousPricedCDR(string carrierAccountID, string isSale, DateTime fromDate, DateTime toDate)
        {
            var data = TABS.DataHelper.ExecuteScalar(@"EXEC [bp_ErroneousPricedCDR]
	                                                        @CarrierAccountID = @P1,
                                                            @IsSale = @P2,    
	                                                        @FromDate = @P3,
	                                                        @TillDate = @P4",
                                                     carrierAccountID, 
                                                     isSale, 
                                                     fromDate, 
                                                     toDate);

            return data;   
        }

        public static IList<TABS.Billing_Invoice> GetInvoice(int? invoiceID)
        {
            string hql = string.Format(@"SELECT bi 
                                             FROM Billing_Invoice bi 
                                             WHERE InvoiceID = :id ");
            IList<TABS.Billing_Invoice> invoice = TABS.DataConfiguration.CurrentSession.CreateQuery(hql)
                .SetParameter("id", invoiceID)
                .SetMaxResults(1)
                .List<TABS.Billing_Invoice>();

            return invoice;
        }
    }
}
