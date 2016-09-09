using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceManager
    {

        #region Public Methods
        public IDataRetrievalResult<InvoiceDetail> GetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            var result = BigDataManager.Instance.RetrieveData(input, new InvoiceRequestHandler(dataManager)) as Vanrise.Entities.BigResult<InvoiceDetail>;
            if(result != null && input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                RecordFilterManager recordFilterManager = new RecordFilterManager();
                var invoiceType = invoiceTypeManager.GetInvoiceType(input.Query.InvoiceTypeId);
                foreach(var data in result.Data)
                { 
                    DataRecordFilterGenericFieldMatchContext context = new DataRecordFilterGenericFieldMatchContext(data.Entity.Details, invoiceType.Settings.InvoiceDetailsRecordTypeId);
                    foreach(var section in invoiceType.Settings.UISettings.SubSections)
                    {
                        if(recordFilterManager.IsFilterGroupMatch(section.FilterGroup, context))
                        {
                            data.SectionTitle = section.SectionTitle;
                        }
                    }
                }
            }
            return result;
        }
        public Entities.Invoice GetInvoice(long invoiceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoice(invoiceId);
        }
        public InsertOperationOutput<InvoiceDetail> GenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<InvoiceDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long insertedInvoiceId = -1;


            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);

            InvoiceGenerationContext context = new InvoiceGenerationContext
            {
                CustomSectionPayload = createInvoiceInput.CustomSectionPayload,
                FromDate = createInvoiceInput.FromDate,
                PartnerId = createInvoiceInput.PartnerId,
                ToDate = createInvoiceInput.ToDate
            };
            invoiceType.Settings.InvoiceGenerator.GenerateInvoice(context);

            Entities.Invoice invoice = new Entities.Invoice
            {
                Details = context.Invoice.InvoiceDetails,
                InvoiceTypeId = createInvoiceInput.InvoiceTypeId,
                FromDate = createInvoiceInput.FromDate,
                PartnerId = createInvoiceInput.PartnerId,
                ToDate = createInvoiceInput.ToDate
            };

            var serialNumber = invoiceType.Settings.SerialNumberPattern;
            InvoiceSerialNumberConcatenatedPartContext serialNumberContext = new InvoiceSerialNumberConcatenatedPartContext{
                Invoice = invoice,
                InvoiceTypeId = createInvoiceInput.InvoiceTypeId
            };
            foreach(var part in invoiceType.Settings.SerialNumberParts)
            {
                if(invoiceType.Settings.SerialNumberPattern != null && invoiceType.Settings.SerialNumberPattern.Contains(string.Format("#{0}#", part.VariableName)))
                {
                    serialNumber = serialNumber.Replace(string.Format("#{0}#", part.VariableName), part.Settings.GetPartText(serialNumberContext));
                }
            }
            invoice.SerialNumber = serialNumber;
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            if (dataManager.SaveInvoices(createInvoiceInput, context.Invoice,invoice,out insertedInvoiceId))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = InvoiceDetailMapper(GetInvoice(insertedInvoiceId));
                insertOperationOutput.Message = "Invoice Generated Successfully.";
                insertOperationOutput.ShowExactMessage = true;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public int GetOverAllInvoiceCount(Guid InvoiceTypeId, string partnerId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetOverAllInvoiceCount( InvoiceTypeId,  partnerId);
        }

        #endregion


        #region Mappers

        private static InvoiceDetail InvoiceDetailMapper(Entities.Invoice invoice)
        {
            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(invoice.InvoiceTypeId);
            string partnerName = null;
            if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.UISettings != null && invoiceType.Settings.UISettings.PartnerSettings.PartnerManagerFQTN != null)
            {
                PartnerManagerContext context = new PartnerManagerContext
                {
                    PartnerId = invoice.PartnerId
                };
                partnerName = invoiceType.Settings.UISettings.PartnerSettings.PartnerManagerFQTN.GetPartnerName(context);
            }
            return new InvoiceDetail
            {
                Entity = invoice,
                PartnerName = partnerName
            };
        }

        #endregion


        #region Private Classes

        private class InvoiceRequestHandler : BigDataRequestHandler<InvoiceQuery, Entities.Invoice, Entities.InvoiceDetail>
        {
            IInvoiceDataManager _dataManager;
            public InvoiceRequestHandler(IInvoiceDataManager dataManager)
            {
                _dataManager = dataManager;
            }
            public override InvoiceDetail EntityDetailMapper(Entities.Invoice entity)
            {
                return InvoiceManager.InvoiceDetailMapper(entity);
            }
            public override IEnumerable<Entities.Invoice> RetrieveAllData(DataRetrievalInput<InvoiceQuery> input)
            {
                return _dataManager.GetGetFilteredInvoices(input);
            }
        }
        #endregion
    }
}
