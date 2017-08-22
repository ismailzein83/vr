using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountDefinitionInfoFilter
    {
        public List<IWHSFinancialAccountDefinitionFilter> Filters { get; set; }
    }
    public interface IWHSFinancialAccountDefinitionFilter
    {
         bool IsMatched(IWHSFinancialAccountDefinitionFilterContext context);
    }
    public interface IWHSFinancialAccountDefinitionFilterContext
    {
        Guid FinancialAccountDefinitionId { get; }
    }
}
