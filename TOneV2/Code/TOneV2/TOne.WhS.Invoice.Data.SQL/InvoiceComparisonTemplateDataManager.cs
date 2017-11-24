using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;

namespace TOne.WhS.Invoice.Data.SQL
{
    public class InvoiceComparisonTemplateDataManager : BaseSQLDataManager, IInvoiceComparisonTemplateDataManager
    {
        public InvoiceComparisonTemplateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        { }
        public List<InvoiceComparisonTemplate> GetInvoiceCompareTemplates()
        {
            return GetItemsSP("[TOneWhS_Invoice].[sp_InvoiceComparisonTemplate_GetAll]", InvoiceCompareTemplateMapper);
        }
        public bool TryAddOrUpdateInvoiceCompareTemplate(InvoiceComparisonTemplate invoiceComparisonTemplate)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_Invoice].[sp_InvoiceComparisonTemplate_TryAddOrUpdate]", invoiceComparisonTemplate.InvoiceTypeId, invoiceComparisonTemplate.PartnerId, Serializer.Serialize(invoiceComparisonTemplate.Details));
            return (recordsEffected > 0);
        }
        public bool AreInvoiceComparisonTemplatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_Invoice.InvoiceComparisonTemplate", ref updateHandle);
        }
        InvoiceComparisonTemplate InvoiceCompareTemplateMapper(IDataReader reader)
        {
            return new InvoiceComparisonTemplate()
            {
                InvoiceComparisonTemplateId= GetReaderValue<long>(reader, "ID"),
                InvoiceTypeId = GetReaderValue<Guid>(reader, "InvoiceTypeId"),
                PartnerId = reader["PartnerId"] as string,
                Details = Vanrise.Common.Serializer.Deserialize<InvoiceComparisonTemplateDetails>(reader["Details"] as string),
             
            };
        }
    }
}
