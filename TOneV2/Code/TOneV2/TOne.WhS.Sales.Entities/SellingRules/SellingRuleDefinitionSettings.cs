using System;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class SellingRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public static Guid CONFIG_ID = new Guid("A249CBD8-EA0D-48F0-A630-AD3B4F6087DE");
        public override Guid ConfigId => CONFIG_ID;
    }
}
