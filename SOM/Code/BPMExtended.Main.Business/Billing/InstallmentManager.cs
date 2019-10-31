using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
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
        public List<InstallmentDetail> GetInstallments(string customerId, string paymentPlanTemplateId, string invoiceCode, string invoiceAmount, string additionalFees, string lateFee, string firstPayment, string reductionRate, string currency, string startDate, string approvalId)
        {
            var input = new SimulateInstallmentInput {
                CustomerId = customerId,
                PaymentPlanTemplateId = paymentPlanTemplateId,
                InvoiceCode = invoiceCode,
                InvoiceAmount = invoiceAmount,
                AdditionalFees = additionalFees,
                LateFee = lateFee,
                FirstPayment = firstPayment,
                ReductionRate = reductionRate,
                Currency = currency,
                StartDate = startDate,
                ApprovalId = approvalId
            };
            var installmentsDetailItems = new List<InstallmentDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<Installment> items = client.Post<SimulateInstallmentInput, List<Installment>>("api/SOM.ST/Billing/SimulateInstallment", input);
                foreach (var item in items)
                {
                    var detailItem = InstallmentToDetailMapper(item);
                    installmentsDetailItems.Add(detailItem);
                }
            }
            return installmentsDetailItems;
        }
        public void ApplyInstallment(string customerId, string paymentPlanTemplateId, string invoiceCode, string invoiceAmount, string additionalFees, string lateFee, string firstPayment, string reductionRate, string currency, string startDate, string approvalId)
        {
            var input = new SimulateInstallmentInput
            {
                CustomerId = customerId,
                PaymentPlanTemplateId = paymentPlanTemplateId,
                InvoiceCode = invoiceCode,
                InvoiceAmount = invoiceAmount,
                AdditionalFees = additionalFees,
                LateFee = lateFee,
                FirstPayment = firstPayment,
                ReductionRate = reductionRate,
                Currency = currency,
                StartDate = startDate,
                ApprovalId = approvalId
            };
            var ProcessInstanceId = new SOMRequestOutput();
            using (SOMClient client = new SOMClient())
            {
                ProcessInstanceId = client.Post<SimulateInstallmentInput, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ApplyInstallment/StartProcess", input);
            }
        }
        public InvoiceAmountData GetInvoiceAmountData(string invoiceId)
        {
            InvoiceAmountData invoiceAmountData = new InvoiceAmountData();
            using (SOMClient client = new SOMClient())
            {
                invoiceAmountData = client.Get<InvoiceAmountData>(String.Format("/api/SOM.ST/Billing/GetInvoiceAmountsForInstallment?invoiceId={0}", invoiceId));
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
