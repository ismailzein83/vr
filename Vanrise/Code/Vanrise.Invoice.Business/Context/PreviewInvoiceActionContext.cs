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
        public int? TimeZoneId { get; set; }
        public void InitializeInvoiceActionContext()
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(this.InvoiceTypeId);
            string offset = null;
            DateTime fromDate = this.FromDate;
            DateTime toDate = this.ToDate;
            if (this.TimeZoneId.HasValue)
            {
                VRTimeZone timeZone = new VRTimeZoneManager().GetVRTimeZone(this.TimeZoneId.Value);
                if (timeZone != null)
                {
                    offset = timeZone.Settings.Offset.ToString();
                    fromDate = this.FromDate.Add(-timeZone.Settings.Offset);
                    toDate = this.ToDate.Add(-timeZone.Settings.Offset);
                }
            }
            PartnerManager partnerManager = new PartnerManager();

            var duePeriod = partnerManager.GetPartnerDuePeriod(this.InvoiceTypeId, this.PartnerId);

            InvoiceGenerationContext context = new InvoiceGenerationContext
            {
                InvoiceTypeId = this.InvoiceTypeId,
                CustomSectionPayload = CustomSectionPayload,
                FromDate = fromDate,
                PartnerId = this.PartnerId,
                IssueDate = this.IssueDate,
                ToDate = toDate,
                GeneratedToDate = this.ToDate,
                DuePeriod = duePeriod
            };
            var invoiceGenerator = invoiceType.Settings.ExtendedSettings.GetInvoiceGenerator();
            invoiceGenerator.GenerateInvoice(context);
            
            this.GeneratedInvoice = context.Invoice;


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
                TimeZoneId = this.TimeZoneId,
                TimeZoneOffset = offset
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
                        Details = invoiceItemWithAdditionalFields,
                        ItemSetName = generatedInvoiceItem.SetName,
                        Name = item.Name,
                    });
                }
               
            }
            return invoiceItems;
        }
    }
}
