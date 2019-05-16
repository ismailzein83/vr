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
        public override Guid ConfigId { get { return new Guid("E9F74D0A-8D26-4F8F-8C0E-97D87904C8BF"); } }

        public override List<string> GetFieldNames()
        {
            return new List<string> { };
        }
    }
}
