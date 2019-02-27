using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Data;

namespace Vanrise.Invoice.Business.Extensions
{
    public class InvoiceMemoryAnalyticDataProvider : MemoryAnalyticDataManager
    {
        static InvoiceTypeManager s_invoiceTypeManager = new InvoiceTypeManager();
        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();
        public Guid InvoiceTypeId { get; set; }

        public override List<RawMemoryRecord> GetRawRecords(Analytic.Entities.AnalyticQuery query, List<string> neededFieldNames)
        {
            return GetInvoiceMemoryRecords(query.FromTime, query.ToTime);
        }
     

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable { get { return true; } }
        }
        private class CashedInvoiceMemoryRecords
        {
            public List<RawMemoryRecord> RawMemoryRecords { get; set; }
            public DateTime FromTime { get; set; }
            public DateTime? ToTime { get; set; }

        }

        #endregion

        #region Private Methods
        private List<RawMemoryRecord> GetInvoiceMemoryRecords(DateTime fromTime, DateTime? toTime)
        {
            var cashedInvoiceMemoryRecords = GetCashedInvoiceMemoryRecords(fromTime, toTime);
            if (fromTime < cashedInvoiceMemoryRecords.FromTime)
            {
               var invoiceMemoryRecordsFromDB =  GetInvoiceMemoryRecordsFromDB(fromTime, cashedInvoiceMemoryRecords.FromTime);
                cashedInvoiceMemoryRecords.RawMemoryRecords.AddRange(invoiceMemoryRecordsFromDB);
                cashedInvoiceMemoryRecords.FromTime = fromTime;

            }
            if(cashedInvoiceMemoryRecords.ToTime.HasValue && toTime > cashedInvoiceMemoryRecords.ToTime)
            {
                var invoiceMemoryRecordsFromDB = GetInvoiceMemoryRecordsFromDB(cashedInvoiceMemoryRecords.ToTime.Value, toTime);
                cashedInvoiceMemoryRecords.RawMemoryRecords.AddRange(invoiceMemoryRecordsFromDB);
                cashedInvoiceMemoryRecords.ToTime = toTime;
            }
            return cashedInvoiceMemoryRecords.RawMemoryRecords;
        }
        private CashedInvoiceMemoryRecords GetCashedInvoiceMemoryRecords(DateTime fromTime, DateTime? toTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCashedInvoiceMemoryRecords",
             () =>
             {
                 return new CashedInvoiceMemoryRecords
                 {
                     FromTime = fromTime,
                     ToTime = toTime,
                     RawMemoryRecords = GetInvoiceMemoryRecordsFromDB(fromTime, toTime)
                 };
             });
        }
        private List<RawMemoryRecord> GetInvoiceMemoryRecordsFromDB(DateTime fromTime, DateTime? toTime)
        {
            var invoiceType = s_invoiceTypeManager.GetInvoiceType(this.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", this.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings", this.InvoiceTypeId);
            var dataRecordTypeFields = s_dataRecordTypeManager.GetDataRecordTypeFields(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            dataRecordTypeFields.ThrowIfNull("dataRecordTypeFields", invoiceType.Settings.InvoiceDetailsRecordTypeId);
            List<RawMemoryRecord> memoryRecords = new List<RawMemoryRecord>();

            IInvoiceDataManager invoiceDataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            var invoices = invoiceDataManager.GetFilteredInvoices(new Vanrise.Entities.DataRetrievalInput<Entities.InvoiceQuery>
            {
                Query = new Entities.InvoiceQuery
                {
                    FromTime = fromTime,
                    ToTime = toTime,
                }
            });

            if (invoices != null && invoices.Count() > 0)
            {
                foreach (var invoice in invoices)
                {
                    InvoiceRecordObject invoiceRecordObject = new InvoiceRecordObject(invoice);

                    RawMemoryRecord rawMemoryRecord = new RawMemoryRecord
                    {
                        FieldValues = new Dictionary<string, dynamic>()
                    };
                    foreach (var field in dataRecordTypeFields)
                    {
                        rawMemoryRecord.FieldValues.Add(field.Key, invoiceRecordObject.InvoiceDataRecordObject.GetFieldValue(field.Key));
                    }
                    memoryRecords.Add(rawMemoryRecord);
                }
            }

            return memoryRecords;
        }
        #endregion
    }
}
