using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
namespace Vanrise.Invoice.Entities
{
    public class InvoiceSubSectionGridColumn
    {
        public string Header { get; set; }
        public string FieldName { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
        public DataRecordFieldType FieldType { get; set; }
    }
}
