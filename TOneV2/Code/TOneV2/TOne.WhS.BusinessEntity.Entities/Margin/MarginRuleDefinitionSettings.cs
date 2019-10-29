using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class MarginRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("9F18087B-F323-4DEE-85BB-60D561789302"); } }

        public Guid MarginCategoryBEDefinitionId { get; set; }
    }
}
