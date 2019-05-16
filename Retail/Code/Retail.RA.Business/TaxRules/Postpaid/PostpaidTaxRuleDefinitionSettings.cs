using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class PostpaidTaxRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("34F5527C-E05D-49BC-9359-8CBD889EB224"); } }

        public override List<string> GetFieldNames()
        {
            return new List<string> { };
        }
    }
}
