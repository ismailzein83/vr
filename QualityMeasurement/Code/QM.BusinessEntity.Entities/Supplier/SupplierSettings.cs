using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public class SupplierSettings
    {
        public List<ExtendedSupplierSetting> ExtendedSettings { get; set; }
    }

    public abstract class ExtendedSupplierSetting
    {
        public abstract void Apply(Supplier supplier);

        public virtual ExcelColumnInfo[] GetExcelColumnNames()
        {
            return null;
        }

        public virtual void ApplyExcelFields(Supplier supplier, Dictionary<string, object> excelFields)
        {

        }
    }

    public class ExcelColumnInfo
    {
        public string ColumnName { get; set; }

        public string[] SampleValues { get; set; }
    }
}
