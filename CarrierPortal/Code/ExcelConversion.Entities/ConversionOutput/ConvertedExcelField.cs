using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class ConvertedExcelField
    {
        public string FieldName { get; set; }

        public Object FieldValue { get; set; }
    }

    public class ConvertedExcelFieldsByName : Dictionary<string, ConvertedExcelField>
    {
        public void Add(ConvertedExcelField fld)
        {
            if (fld == null)
                throw new ArgumentNullException("fld");
            this.Add(fld.FieldName, fld);
        }
    }
}
