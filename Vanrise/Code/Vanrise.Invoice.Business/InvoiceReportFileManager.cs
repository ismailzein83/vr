using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;

namespace Vanrise.Invoice.Business
{
    public class InvoiceReportFileManager
    {
        public string GetInvoiceReportFileName(Guid invoiceReportFileId)
        {
            GenericBusinessEntityManager genericBEManager = new GenericBusinessEntityManager();
            var invoiceFileReportBE = genericBEManager.GetGenericBusinessEntity(invoiceReportFileId, new Guid("64f8db86-691d-4486-83fb-26a3d3fc095e"));
            if (invoiceFileReportBE != null && invoiceFileReportBE.FieldValues!=null && invoiceFileReportBE.FieldValues.Count>0)
            {
                var reportName =  invoiceFileReportBE.FieldValues.GetRecord("ReportName");
                return reportName != null ? reportName.ToString() : null;
            }
            return null;
        }
    }
}
