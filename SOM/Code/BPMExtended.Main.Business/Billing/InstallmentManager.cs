using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class InstallmentManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }


        #endregion
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

        public List<Installment> GetPaymentPlanInstallments(string invoiceId, string paymentPlanId)
        {
            List<Installment> item = null;
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<List<Installment>>(String.Format("api/SOM.ST/Billing/GetPaymentPlanInstallments?invoiceId={0}&paymentPlanId={1}", invoiceId, paymentPlanId));

            }
            return item;
        }

        public void ApplyInstallment(Guid requestId/*,string customerId, string paymentPlanTemplateId, string invoiceCode, string invoiceAmount, string additionalFees, string lateFee, string firstPayment, string reductionRate, string currency, string startDate, string approvalId*/)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StInstallments");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StTemplateId");
            esq.AddColumn("StInvoiceCode");
            esq.AddColumn("StInvoiceAmount");
            esq.AddColumn("StAdditionalFees");
            esq.AddColumn("StLateFees");
            esq.AddColumn("StFirstPayment");
            esq.AddColumn("StCurrency");
            esq.AddColumn("StReductionRate");
            esq.AddColumn("StStartDate");
            esq.AddColumn("StContractID");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                string customerId = entities[0].GetColumnValue("StCustomerId").ToString();
                string paymentPlanTemplateId = entities[0].GetColumnValue("StTemplateId").ToString();
                string invoiceCode = entities[0].GetColumnValue("StInvoiceCode").ToString();
                string invoiceAmount = entities[0].GetColumnValue("StInvoiceAmount").ToString();
                string additionalFees = entities[0].GetColumnValue("StAdditionalFees").ToString();
                string lateFee = entities[0].GetColumnValue("StLateFees").ToString();
                string reductionRate = entities[0].GetColumnValue("StReductionRate").ToString();
                string startDate = entities[0].GetColumnValue("StStartDate") != null ? entities[0].GetColumnValue("StStartDate").ToString() : null;
                string currency = entities[0].GetColumnValue("StCurrency").ToString();
                string firstPayment = entities[0].GetColumnValue("StFirstPayment").ToString();
                string contractId = entities[0].GetColumnValue("StContractID").ToString();

                SOMRequestInput<SimulateInstallment> input = new SOMRequestInput<SimulateInstallment>
                {

                    InputArguments = new SimulateInstallment
                    {
                        input = new SimulateInstallmentInput
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
                            ApprovalId = "1"
                        }
                    }

                };

                var ProcessInstanceId = new SOMRequestOutput();
                using (SOMClient client = new SOMClient())
                {
                    ProcessInstanceId = client.Post<SOMRequestInput<SimulateInstallment>,SOMRequestOutput>("api/DynamicBusinessProcess_BP/ApplyInstallment/StartProcess", input);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, ProcessInstanceId);
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
                Date = item.DueDate.ToString("MM/dd/yyyy")

            };
        }
    }
}
