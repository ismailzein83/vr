using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    public abstract class TargetMatchCalculationMethod
    {
        public abstract Guid ConfigId { get; }

        public abstract void Evaluate(ITargetMatchCalculationMethodContext context);
    }

    public class SupplierTargetMatchMethodConfig : ExtensionConfiguration
    {

        public const string EXTENSION_TYPE = "WhS_Sales_SupplierTargetMatchMethod";
        public string Editor { get; set; }
    }
}
