using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountTypeBESelectorFilter : BERuntimeSelectorFilter
    {
        public Guid AccountBEDefinitionId { get; set; }

        public override bool IsMatched(IBERuntimeSelectorFilterSelectorFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
