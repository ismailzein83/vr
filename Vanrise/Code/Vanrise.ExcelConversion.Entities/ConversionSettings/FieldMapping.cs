using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public enum FieldType { String = 0, DateTime = 1, Decimal = 2, Int = 3 }

    public abstract class FieldMapping
    {
        public virtual Guid ConfigId { get; set; }

        public string FieldName { get; set; }

        public FieldType FieldType { get; set; }

        public abstract object GetFieldValue(IGetFieldValueContext context);
    }
}
