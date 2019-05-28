using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.SOMAPI;

namespace BPMExtended.Main.Business
{
    public class PaymentManager
    {

        #region Public
        public List<PaymentPlanDetail> GetPaymentPlansByInvoiceId(string invoiceId)
        {
                var paymentDetails = new List<PaymentPlanDetail>();
                using (SOMClient client = new SOMClient())
                {
                    List<PaymentPlan> items = client.Get<List<PaymentPlan>>(String.Format("api/SOM.ST/Billing/GetInvoiceInstallments?InvoiceId={0}", invoiceId));
                    foreach (var item in items)
                    {
                        var paymentPlanDetail = PaymentPlanToDetailMapper(item);
                        paymentDetails.Add(paymentPlanDetail);
                    }
                }
                return paymentDetails;
        }
        #endregion

        #region Mappers
        public PaymentPlanDetail PaymentPlanToDetailMapper(PaymentPlan item)
        {
            return new PaymentPlanDetail()
            {
                Id = item.Id,
                Desc = item.Desc,
                Amount = Convert.ToString(item.Amount),
                EndDate = Convert.ToString(item.EndDate),
                NextInstallmentAmount = Convert.ToString(item.NextInstallmentAmount),
                NextInstallmentDueDate = Convert.ToString(item.NextInstallmentDueDate),
                NextInstallmentInterest = Convert.ToString(item.NextInstallmentInterest),
                StartDate = Convert.ToString(item.StartDate),
                Status = Convert.ToString(item.Status)
            };
        }
        #endregion
    }
}
