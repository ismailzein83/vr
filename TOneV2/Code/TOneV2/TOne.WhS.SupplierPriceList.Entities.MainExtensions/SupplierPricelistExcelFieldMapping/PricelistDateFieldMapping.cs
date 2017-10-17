using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.ExcelConversion.Entities;

namespace TOne.WhS.SupplierPriceList.MainExtensions
{
    public class PricelistDateFieldMapping : FieldMapping
    {
        public override Guid ConfigId { get { return new Guid("13CDF53A-3780-41D3-9FE1-0054458171C3"); } }
        public override object GetFieldValue(IGetFieldValueContext context)
        {
            ImportSupplierPriceListExtendedSettings extendedSettings = context.ExtendedSettings as ImportSupplierPriceListExtendedSettings;
            if (extendedSettings == null)
                throw new Exception("extendedSettings should be of type ImportSupplierPriceListExtendedSettings");

            return extendedSettings.PricelistDate;
        }
    }
}
