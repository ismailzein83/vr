using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.Business
{
    public enum RecurringChargeType { Customer = 0, Supplier = 1 }
    public class RecurringChargeTypeSettings : GenericBEExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("2289C6CB-81B0-449C-9063-1AD156450B18"); } }

        public RecurringChargeType Type { get; set; }
    }
}
