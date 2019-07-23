using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class PrepaidTaxRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("A95A04B7-232F-4D5F-9EAB-8ADFAADB89AA"); } }
        public override List<string> GetFieldNames()
        {
            List<string> fieldNames = new List<string>();
            fieldNames.Add("TotalTopUpAmount");
            fieldNames.Add("TotalTopUpTaxValue");
            return fieldNames;
        }
    }
}
