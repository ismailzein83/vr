using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class InstallmentManager
    {
        public List<InstallmentTemplateInfo> GetInstallmentTemplatesInfo()
        {
            var installmentTemplateInfoItems = new List<InstallmentTemplateInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<InstallmentTemplate> items = client.Get<List<InstallmentTemplate>>(String.Format("api/SOM.ST/Billing/GetPaymentPlanTemplates"));
                foreach (var item in items)
                {
                    var customerCatergoryInfoItem = InstallmentTemplateToInfoMapper(item);
                    installmentTemplateInfoItems.Add(customerCatergoryInfoItem);
                }
            }
            return installmentTemplateInfoItems;
        }
        public List<InstallmentDetail> GetInstallments(string customerId,string paymentPlanTemplateId,string invoiceCode,string invoiceAmount,string additionalFees,string lateFee,string firstPayment,string reductionRate,string currency,string startDate,string approvalId)
        {

            var installmentsDetailItems = new List<InstallmentDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<Installment> items = client.Get<List<Installment>>(String.Format("api/SOM.ST/Billing/SimulateInstallment?CustomerId={0}&PaymentPlanTemplateId={1}&InvoiceId={2}&InvoiceAmount={3}&AdditionalFees={4}&LateFee={5}&FirstPayment={6}&ReductionRate={7}&Currency={8}&StartDate={9}&ApprovalId={10}"
                    , customerId, paymentPlanTemplateId, invoiceCode, invoiceAmount, additionalFees, lateFee, firstPayment, reductionRate, currency, startDate, approvalId));
                foreach (var item in items)
                {
                    var detailItem = InstallmentToDetailMapper(item);
                    installmentsDetailItems.Add(detailItem);
                }
            }
            return installmentsDetailItems;
        }
        public InvoiceAmountData GetInvoiceAmountData(string invoiceId)
        {
            InvoiceAmountData invoiceAmountData = new InvoiceAmountData();
            using (SOMClient client = new SOMClient())
            {
                 invoiceAmountData = client.Get<InvoiceAmountData>(String.Format("/api/SOM.ST/Billing/GetInvoiceAmountsForInstallment?invoiceId={0}",invoiceId));
            }
            return invoiceAmountData;
        }
        public InstallmentTemplateInfo InstallmentTemplateToInfoMapper(InstallmentTemplate item)
        {
            return new InstallmentTemplateInfo
            {
                Name = item.Name,
                Id = item.Id

            };
        }
        public InstallmentDetail InstallmentToDetailMapper(Installment item)
        {
            return new InstallmentDetail
            {
               Id = item.Id,
               Amount = item.Amount,
               Currency = item.Currency,
               Date = item.Date

            };
        }
    }
}
