using System;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class MarginRateCalculationConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Sales_MarginRateCalculation";

        public string Editor { get; set; }
    }
}
