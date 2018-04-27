using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Data;

namespace Vanrise.Invoice.Business
{
    public class InvoiceBusinessObjectDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public Guid InvoiceTypeId { get; set; }

        static Vanrise.GenericData.Business.RecordFilterManager s_recordFilterManager = new GenericData.Business.RecordFilterManager();
        static DataRecordTypeManager s_recordTypeManager = new DataRecordTypeManager();
        static InvoiceTypeManager s_invoiceTypeManager = new InvoiceTypeManager();
        static IInvoiceDataManager s_invoiceDataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();

        DataRecordType _dataRecordType;

        public override Guid ConfigId
        {
            get { return new Guid("50D14DDE-3AB4-48BE-AA14-0242ADCC872F"); }
        }

        public override bool DoesSupportFilterOnAllFields
        {
            get { return false; }
        }

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            RecordFilterGroup filterGroupToSendToDataManager = context.FilterGroup != null ? PrepareFilterGroupToSendToDataManager(context.FilterGroup) : null;
            Func<bool> shouldStopFunc = () => context.IsLoadStopped;

            s_invoiceDataManager.LoadInvoices(this.InvoiceTypeId, context.FromTime, context.ToTime, filterGroupToSendToDataManager, context.OrderDirection,
                shouldStopFunc,
                (invoice) =>
                {
                    InvoiceRecordObject invoiceRecordObject = new InvoiceRecordObject(invoice);
                    context.OnRecordLoaded(invoiceRecordObject.InvoiceDataRecordObject, invoice.CreatedTime);
                });
        }

        private DataRecordType GetDataRecordTypeWithValidate()
        {
            if (_dataRecordType == null)
            {
                var invoiceType = s_invoiceTypeManager.GetInvoiceType(this.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", this.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", this.InvoiceTypeId);
                _dataRecordType = s_recordTypeManager.GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
                _dataRecordType.ThrowIfNull("_dataRecordType", invoiceType.Settings.InvoiceDetailsRecordTypeId);
                _dataRecordType.Fields.ThrowIfNull("_dataRecordType.Fields", invoiceType.Settings.InvoiceDetailsRecordTypeId);
            }
            return _dataRecordType;
        }

        RecordFilterGroup PrepareFilterGroupToSendToDataManager(RecordFilterGroup recordFilter)
        {
            DataRecordType invoiceRecordType = GetDataRecordTypeWithValidate();
            List<string> fieldsToExcludeFromDBQuery = invoiceRecordType.Fields.Select(itm => itm.Name).ToList();
            return s_recordFilterManager.ReBuildRecordFilterGroupWithExcludedFields(recordFilter, fieldsToExcludeFromDBQuery);
        }
    }
}
