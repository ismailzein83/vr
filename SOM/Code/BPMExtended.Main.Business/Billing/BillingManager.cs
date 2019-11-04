using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using SOM.Main.BP.Arguments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Configuration;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;
using PdfSharp;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Xml.XPath;
using MigraDoc.Rendering;
using System.Diagnostics;
using System.IO;
using SOM.Main.Entities;
using BPMExtended.Main.SOMAPI;
using System.Xml.Serialization;

namespace BPMExtended.Main.Business
{
    public class BillingManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        Document document = new Document();
        MigraDoc.DocumentObjectModel.Shapes.TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;


        public CustomerBalance GetCustomerBalance(string customerId)
        {
            return RatePlanMockDataGenerator.GetCustomerBalance(customerId);
        }
        public bool ExcludeInvoiceFromCollection(string invoiceId)
        {
            return true;
        }

        public bool UpdateInvoiceComplaintFlag(string invoiceId, bool status)
        {
            return true;
        }


        public List<InvoiceFee> GetInvoicesFees(string contracId, string requestId , List<string> services)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            List<InvoiceFee> fees;
            string ratePlanId = null;

            //string operationId = new CommonManager().GetOperationByRequestId(requestId);
            //string entityName = new CommonManager().GetEntityNameByOperationId(operationId);


            //if (entityName == "StLineSubscriptionRequest" || entityName == "StADSL" || entityName == "StGSHDSL" || entityName == "StLeasedLine")
            //{ 
            //    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, entityName);
            //    esq.AddColumn("StRatePlanID");

            //    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", contracId);
            //    esq.Filters.Add(esqFirstFilter);
            //    entities = esq.GetEntityCollection(BPM_UserConnection);

            //    if (entities.Count > 0)
            //    {
            //        ratePlanId = entities[0].GetColumnValue("StRatePlanID").ToString();

            //    }
            //}
            //else
            //{
                ratePlanId = new ContractManager().GetContractDetails(contracId).RateplanId;
            //}

            var input = new InvoiceFeeInput()
            {
                RatePlanId = ratePlanId,
                ServiceIds = services
            };
          
            using (SOMClient client = new SOMClient())
            {
                fees = client.Post<InvoiceFeeInput, List<InvoiceFee>>("api/SOM.ST/Billing/GetInvoiceFees", input);
            }

            return fees;
        }


        public bool CheckIfUserPayForWaitingList(Guid requestId)
        {

            //Get sequence Number
            string sequenceNumber = new CRMCustomerManager().GetSequenceNumberFromRequestHeader(requestId);

            using (SOMClient client = new SOMClient())
            {
                return client.Get<bool>(String.Format("api/SOM.ST/Billing/CheckIfUserPayForWaitingList?requestId={0}",sequenceNumber));
            }

            //TODO: check if user paid 
            //return new Random().Next(10) <= 5 ? true : false;

        }

        public decimal SubmitToPOS(string contractId, string requestId, BPMExtended.Main.Entities.OperationType operationType)
        {
            //Get from BPM the list of core services mapped to this operation type
            //Send to POS the list of services to pay with the contract id

            return 3500;
        }
        public PaymentInfo SubmitCallDetailsToPOS(string contractId, string numberOfRecords, string requestId, string customerId)
        {
            object pagesize = "";
            SysSettings.TryGetValue(this.BPM_UserConnection, "CDR_Page_Size", out pagesize);
            
            decimal pgsize = 0;
            decimal.TryParse(pagesize.ToString(), out pgsize);
            decimal number = 0;
            decimal.TryParse(numberOfRecords, out number);

            decimal pages = (number / pgsize);

            PaymentInfo paymentinfo = new PaymentInfo()
            {
                amountToPay = (Math.Ceiling(pages) * 1000),
            };
            return paymentinfo;
        }

        public List<BillCycle> GetBillCycles()
        {
            List<BillCycle> items = new List<BillCycle>();

            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<BillCycle>>(String.Format("api/SOM.ST/Billing/GetBillCycles?processInstanceId"));
            }
            return items;
        }


      /*  public List<BillCycle> GetBillCycles()
        {
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddYears(-2);


            List<BillCycle> billCycles = new List<BillCycle>();
            int i = 1;
            while (endDate <= startDate)
            {
                if (endDate.Month == 1 || endDate.Month == 4 || endDate.Month == 7 || endDate.Month == 10)
                {
                    string dt = endDate.Month + ", " + endDate.Year;
                    BillCycle bs = new BillCycle() { Id = i, Title = dt };
                    billCycles.Add(bs);
                    i++;
                }
                endDate = endDate.AddMonths(1);
            }
            return billCycles;
        }*/
       
        public bool ValidatePaymentWithoutDeposit(string requestId)
        {
            //TODO: check if user has paid
            return true;
        }

        public bool isUserNameUnique(string userName)
        {
            //TODO: call BSCS to validate the username
            bool result;

            using (SOMClient client = new SOMClient())
            {
                result = client.Get<bool>(String.Format("api/SOM.ST/Billing/IsUsernameAvailable?username={0}", userName));
            }

            return result;
        }


        public bool GetPABXTree(string contractId)
        {
            List<string> result;

            using (SOMClient client = new SOMClient())
            {
                result = client.Get<List<string>>(String.Format("api/SOM.ST/Billing/GetPABXTree?contractId={0}", contractId));
            }

            return result.Count > 0;
        }

        public bool generateInvoice(string customerId , string contractId , bool simulate)
        {
            //TODO: generate Invoice 
            bool result;

            using (SOMClient client = new SOMClient())
            {
                result = client.Get<bool>(String.Format("api/SOM.ST/Billing/BillOnDemand?customerId={0}&contractId={1}&simulate={2}", customerId , contractId , simulate));
            }

            return result;
        }

        public bool BillOnDemandGenerateInvoice(string customerId, string contractId, bool simulate)
        {
            //TODO: generate Invoice 
            bool result;

            using (SOMClient client = new SOMClient())
            {
                result = client.Get<bool>(String.Format("api/SOM.ST/Billing/BillOnDemand?customerId={0}&contractId={1}&simulate={2}", customerId, contractId, simulate));
            }

            return result;
        }


        public bool validateInvoice(string invoiceId)
        {
            //TODO: 

            return true;
        }


        public bool addTaxesOnInvoice()
        {
            //TODO : add the taxes of the operation on the invoice as OCC
            //TODO :check also if VPN service is active on the contract then CRM should add fee to the contract 

            return true;
        }




        public List<CreditDebitNotesDetail> GetCreditDebitNotes(string customerId)
        {
            var creditDebitNotesItems = new List<CreditDebitNotesDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<CreditDebitNotes> items = client.Get<List<CreditDebitNotes>>(String.Format("api/SOM.ST/Billing/GetCustomerDebitAndCreditNotes?CustomerId={0}", customerId));
                foreach (var item in items)
                {
                    var detailItem = CreditDebitNotesMapper(item);
                    creditDebitNotesItems.Add(detailItem);
                }
            }
            return creditDebitNotesItems;
        }

        public List<PromotionDetail> GetPromotions(string contractId)
        {
            var promotionsItems = new List<PromotionDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<PromotionPackage> items = client.Get<List<PromotionPackage>>(String.Format("api/SOM.ST/Billing/GetContractPromotions?ContractId={0}", contractId));
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var detailItem = PromotionToDetailMapper(item);
                        promotionsItems.Add(detailItem);
                    }
                }
            }
            return promotionsItems;
        }




        public List<PaymentDetail> GetPayments(string customerId)
        {
            // return RatePlanMockDataGenerator.GetPayments(customerId);
            var paymentDetailItems = new List<PaymentDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<Payment> items = client.Get<List<Payment>>(String.Format("api/SOM.ST/Billing/GetCustomerPayments?CustomerId={0}", customerId));
                foreach (var item in items)
                {
                    var detailItem = PaymentToDetailMapper(item);
                    paymentDetailItems.Add(detailItem);
                }
            }
            return paymentDetailItems;
        }



        public PaymentPlan GetPaymentPlanById(string paymentPlanId)
        {
            return RatePlanMockDataGenerator.GetPaymentPlanById(paymentPlanId);
        }

        public List<PaymentPlanTemplateInfo> GetAllPaymentPlanTemplatesInfo()
        {
            var templatesInfoItems = new List<PaymentPlanTemplateInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<PaymentPlanTemplate> items = client.Get<List<PaymentPlanTemplate>>(String.Format("api/SOM.ST/Billing/GetPaymentPlanTemplates"));
                foreach (var item in items)
                {
                    var templateInfoItem = TemplateToInfoMapper(item);
                    templatesInfoItems.Add(templateInfoItem);
                }
            }
            return templatesInfoItems;
        }

        public List<ProfessionalDI> GetFilteredProfessionalDI(string name)
        {
            List<ProfessionalDI> items = new List<ProfessionalDI>();
            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<ProfessionalDI>>(String.Format("api/SOM.ST/Billing/FilterPrivateDirectoryInquiry?businessType=&givenName={0}",name));
            }
            return items;
            //return RatePlanMockDataGenerator.GetFilteredProfessionalDI(firstName,fatherName,lastName,street,province,city,profession);
        }

        public List<NormalDI> GetFilteredNormalDI(string firstName, string fatherName, string lastName, string enterpriseName)
        {
            List<NormalDI> items = new List<NormalDI>();
            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<NormalDI>>(String.Format("api/SOM.ST/Billing/FilterPublicDirectoryInquiry?firstName={0}&lastName={1}&middleName={2}&enterpriseName={3}",
                    firstName,fatherName,lastName,enterpriseName));
                
            }
            return items;
            //return RatePlanMockDataGenerator.GetFilteredNormalDI(firstName, fatherName, lastName, enterpriseName);
        }

        public List<CompanyDI> GetFilteredCompaniesDI(string accountName, string city, string province, string street)
        {
            return RatePlanMockDataGenerator.GetFilteredCompaniesDI(accountName, city, province, street);
        }

        public bool InstallmentsApproval(string templateId, string invoiceId, string discount)
        {
            return true;

        }

        public bool CancelInvoiceInstallment(string templateId, string invoiceId, string paymentPlanId)
        {
            return true;

        }

        public bool GenerateBillOnDemand(string billingType)
        {
            return true;

        }


        public string GenerateBillOnDemand(string customerId, string contractId, bool simulate)
        {
            string processInstanceId = string.Empty;
            var commonInputArgument = new CommonInputArgument()
            {
                CustomerId = customerId,
                ContractId = contractId
            };
            var billOnDemandInputArguments = new BillOnDemandInputArguments()
            {
                CommonInputArgument = commonInputArgument,
                Simulate = simulate
            };

            var billOnDemandInput = new BillOnDemandInput()
            {
                InputArguments = billOnDemandInputArguments
            };

            var ProcessInstanceId =new SOMRequestOutput();
            using (SOMClient client = new SOMClient())
            {
                ProcessInstanceId = client.Post<BillOnDemandInput, SOMRequestOutput>("api/DynamicBusinessProcess_BP/BillOnDemand/StartProcess", billOnDemandInput);
            }



            return ProcessInstanceId.ProcessId;
        }

        public List<PaymentMethodInfo> ReadPaymentMethodsInfo()
        {
            var paymentMethodsInfoItems = new List<PaymentMethodInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<PaymentMethodInfo> items = client.Get<List<PaymentMethodInfo>>(String.Format("api/SOM.ST/Billing/GetPaymentMethods"));
                foreach (var item in items)
                {
                    paymentMethodsInfoItems.Add(item);
                }
            }
            return paymentMethodsInfoItems;

        }
        public static string SerializeToString(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);

                return writer.ToString();
            }
        }


        public bool GetCollectionStatus(string invoiceId)
        {
            //TODO:Get collection status by invoice Id
            Random value = new Random();
            return value.Next(10) <= 5 ? true : false;
        }

        public bool GetInvoiceInstallmentFlag(string invoiceId)
        {
            //TODO:Get installment flag by invoice Id
            Random value = new Random();
            return value.Next(10) <= 5 ? true : false;
        }

        public bool GetFinancialDisputes(string invoiceId)
        {
            //TODO: Get financial disputes requests related to this invoice
            Random value = new Random();
            return value.Next(10) <= 5 ? true : false;
        }

        #region Mappers

        

        public PaymentPlanTemplateInfo TemplateToInfoMapper(PaymentPlanTemplate item)
        {
            return new PaymentPlanTemplateInfo
            {
                Id = item.Id,
                Name = item.Name

            };
        }

        public PaymentDetail PaymentToDetailMapper(Payment item)
        {
            string paymentDate= null;
            if (item.EntryDate != null)
            {
                paymentDate = item.EntryDate.ToString();
            }
            return new PaymentDetail
            {
                PaymentCode = item.Code,
                PaymentDate = paymentDate,
                CashierUserName = item.GLAccount,
            };
        }

        public CreditDebitNotesDetail CreditDebitNotesMapper(CreditDebitNotes item)
        {
            return new CreditDebitNotesDetail
            {
                DocumentId = item.Id,
                DocType = item.DocumentType,
                BillingAccountCode = item.BillingAccountCode,
                IssueDate = Convert.ToString(item.EntryDate),
                DocumentAmount = Convert.ToString(item.Amount),
                DocumentOpenAmount = Convert.ToString(item.OpenAmount)
               // IssueDate = item.EntryDate.ToString(),
               // DocumentAmount = item.Amount.ToString(),
               // DocumentOpenAmount = item.OpenAmount.ToString()
            };
        }

        public PromotionDetail PromotionToDetailMapper(PromotionPackage item)
        {
            return new PromotionDetail()
            {
                PromotionPackageId = item.PromotionPackageId,
                AssignDate = Convert.ToString(item.AssignDate)
            };
        }

        public CallDetailsEntity CallDetailsToEntityMapper(CallDetails callDetails)
        {
            return new CallDetailsEntity()
            {
                Amount = callDetails.Amount,
                Date = callDetails.CallDate,
                Duration = callDetails.Duration,
                Number = callDetails.CalledNumber,
                Place = callDetails.CallingNumber //To check Place
            };
        }

        #endregion

    }



    public class PDFDocument
    {

        public string Id { get; set; }
        public byte[] FileData { get; set; }
        public int pageCount { get; set; }

    }
    public class PaymentInfo
    {

        public decimal amountToPay { get; set; }
        public bool isForeigner { get; set; }

        public bool isContractCreated { get; set; }

        public decimal depositAmount { get; set; }

        public string depositId { get; set; }


    }
    public class BillCycle
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IntervalType { get; set; }
        public DateTime RunDate { get; set; }
    }
    public class CallDetailRequest
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string CSO { get; set; }
        public string BillCycle { get; set; }
        public string Date { get; set; }
        public string PhoneNumber { get; set; }
        public List<CallDetails> Items { get; set; }
    }
    public class CallDetailsEntity
    {
        public DateTime Date { get; set; }
        public Decimal Duration { get; set; }
        public string Place { get; set; }
        public string Number { get; set; }
        public Decimal Amount { get; set; }

    }
    public class CallDetails
    {
        public string Id { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }
        public string DurationUnit { get; set; }
        public Decimal Duration { get; set; }
        public Decimal Amount { get; set; }
        public DateTime CallDate { get; set; }
    }



    #region commented
    //public List<PaymentPlanDetail> GetPaymentPlansByInvoiceId(string invoiceId)
    //{
    //    return RatePlanMockDataGenerator.GetPaymentPlansByInvoiceId(invoiceId);
    //}

    //public Invoice GetInvoiceById(string invoiceId)
    //{
    //    var item = new Invoice();
    //    using (SOMClient client = new SOMClient())
    //    {
    //        item = client.Get<Invoice>(String.Format("api/SOM.ST/Billing/GetInvoiceDetails?InvoiceId={0}", invoiceId));
    //    }
    //    return item;
    //}

    //public List<InvoiceDetail> GetInvoices(string customerId)
    //{
    //    //TODO:Get invoices
    //    // List<InvoiceDetail> invoices =  RatePlanMockDataGenerator.GetInvoices(customerId);

    //    //foreach (InvoiceDetail invoice in invoices)
    //    //{
    //    //    invoice.CollectionStatus = GetCollectionStatus(invoice.InvoiceCode);
    //    //    invoice.InvoiceInstallmentFlag = GetInvoiceInstallmentFlag(invoice.InvoiceId);
    //    //    invoice.FinancialDisputes = GetFinancialDisputes(invoice.InvoiceId);
    //    //}
    //    //            return invoices;

    //    var invoicesDetailItems = new List<InvoiceDetail>();
    //    using (SOMClient client = new SOMClient())
    //    {
    //        List<Invoice> items = client.Get<List<Invoice>>(String.Format("api/SOM.ST/Billing/GetCustomerInvoices?CustomerId={0}", customerId));
    //        foreach (var item in items)
    //        {
    //            var detailItem = InvoiceToDetailMapper(item);
    //            invoicesDetailItems.Add(detailItem);
    //        }
    //    }
    //    return invoicesDetailItems;
    //}

    //public InvoiceDetail InvoiceToDetailMapper(Invoice item)
    //{//Invoice Code, Bill Cycle, phone Number , InvoiceAmount, open amount, Invoice URL
    //    return new InvoiceDetail
    //    {
    //        InvoiceCode = item.Id,
    //        InvoiceAccount = item.BillingAccountCode,
    //        OpenAmount = Convert.ToString(item.OpenAmount),
    //        Amount = Convert.ToString(item.Amount)
    //    };
    //}

    #endregion

}
