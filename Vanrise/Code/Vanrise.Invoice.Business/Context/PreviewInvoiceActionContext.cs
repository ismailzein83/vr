using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;
using Vanrise.Common;
using Vanrise.Security.Entities;
namespace Vanrise.Invoice.Business
{
    public class PreviewInvoiceActionContext : IInvoiceActionContext
    {
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime IssueDate { get; set; }
        public dynamic CustomSectionPayload { get; set; }
        private bool IsLoaded { get; set; }
        public void InitializeInvoiceActionContext()
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(this.InvoiceTypeId);
          
            PartnerManager _partnerManager = new PartnerManager();

            var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceType.InvoiceTypeId, this.PartnerId);
            invoiceAccountData.ThrowIfNull("invoiceAccountData");
            if ((invoiceAccountData.BED.HasValue && this.FromDate < invoiceAccountData.BED.Value) || (invoiceAccountData.EED.HasValue && this.ToDate > invoiceAccountData.EED.Value))
                throw new Exception("From date and To date should be within the effective date of invoice account.");

            PartnerManager partnerManager = new PartnerManager();

            var duePeriod = partnerManager.GetPartnerDuePeriod(this.InvoiceTypeId, this.PartnerId);





            InvoiceGenerationContext context = new InvoiceGenerationContext
            {
                InvoiceTypeId = this.InvoiceTypeId,
                CustomSectionPayload = CustomSectionPayload,
                FromDate = this.FromDate,
                PartnerId = this.PartnerId,
                IssueDate = this.IssueDate,
                ToDate = this.ToDate,
                DuePeriod = duePeriod
            };



            var invoiceGenerator = invoiceType.Settings.ExtendedSettings.GetInvoiceGenerator();
            invoiceGenerator.GenerateInvoice(context);


            switch(context.GenerateInvoiceResult)
            {
                case GenerateInvoiceResult.Succeeded:
                    break;
                case GenerateInvoiceResult.Failed:
                case GenerateInvoiceResult.NoData:
                    throw new Exception(context.ErrorMessage != null ? context.ErrorMessage : "No data available between the selected period.");
            }

            this.GeneratedInvoice = context.Invoice;

            GeneratedInvoice.ThrowIfNull("GeneratedInvoice");
           // var serialNumber = new PartnerManager().GetPartnerSerialNumberPattern(this.InvoiceTypeId, this.PartnerId);

            this._Invoice = new Entities.Invoice
            {
                Details = GeneratedInvoice.InvoiceDetails,
                ToDate = this.ToDate,
                PartnerId = this.PartnerId,
                FromDate = this.FromDate,
                InvoiceTypeId = this.InvoiceTypeId,
                IssueDate = this.IssueDate,
                DueDate = this.IssueDate.AddDays(duePeriod),
                
            };


            InvoiceSerialNumberConcatenatedPartContext serialNumberContext = new InvoiceSerialNumberConcatenatedPartContext
            {
                Invoice = this._Invoice,
                InvoiceTypeId = this.InvoiceTypeId
            };
            //foreach (var part in invoiceType.Settings.InvoiceSerialNumberSettings.SerialNumberParts)
            //{
            //    if (serialNumber != null && serialNumber.Contains(string.Format("#{0}#", part.VariableName)))
            //    {
            //        serialNumber = serialNumber.Replace(string.Format("#{0}#", part.VariableName), part.Settings.GetPartText(serialNumberContext));
            //    }
            //}

            //this._Invoice.SerialNumber = serialNumber;
           
            this.IsLoaded = true;
        }
        public GeneratedInvoice GeneratedInvoice { get; set; }
        private Entities.Invoice _Invoice { get; set; }
        public Entities.Invoice GetInvoice
        {
            get
            {
                if (!this.IsLoaded)
                    InitializeInvoiceActionContext();
                return this._Invoice;
            }
        }
        public IEnumerable<InvoiceItem> GetInvoiceItems(List<string> itemSetNames, CompareOperator compareOperator)
        {
            if (!this.IsLoaded)
                InitializeInvoiceActionContext();
            
            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();



            var generatedInvoiceItems = this.GeneratedInvoice.InvoiceItemSets.Where(x =>{
                switch(compareOperator)
                {
                    case CompareOperator.StartWith: return itemSetNames.Any(y => x.SetName.StartsWith(y)); 
                    case CompareOperator.Contains: return itemSetNames.Any(y => x.SetName.Contains(y)); 
                    case CompareOperator.EndWith: return itemSetNames.Any(y => x.SetName.EndsWith(y));
                    case CompareOperator.Equal :  return itemSetNames.Any(y=>  x.SetName.Equals(y));
                }
                return false;
            });
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(this.InvoiceTypeId);
            InvoiceItemAdditionalFieldsContext context = new InvoiceItemAdditionalFieldsContext
            {
                InvoiceType = invoiceType
            };

            foreach (var generatedInvoiceItem in generatedInvoiceItems)
            {
              
                foreach (var item in generatedInvoiceItem.Items)
                {
                    var invoiceItemWithAdditionalFields = item.Details as IInvoiceItemAdditionalFields;
                    if (invoiceItemWithAdditionalFields != null)
                        invoiceItemWithAdditionalFields.FillAdditionalFields(context);
                    invoiceItems.Add(new InvoiceItem
                    {
                        Details = invoiceItemWithAdditionalFields != null ? invoiceItemWithAdditionalFields : item.Details,
                        ItemSetName = generatedInvoiceItem.SetName,
                        Name = item.Name,
                    });
                }
               
            }
            return invoiceItems;
        }

        public bool DoesUserHaveAccess(Guid invoiceActionId)
        {
            return new InvoiceTypeManager().DosesUserHaveActionAccess(InvoiceActionType.Download,  this.InvoiceTypeId, invoiceActionId);
        }

    }
}
