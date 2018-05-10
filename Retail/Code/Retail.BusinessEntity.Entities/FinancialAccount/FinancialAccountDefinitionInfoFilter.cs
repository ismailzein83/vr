using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccountDefinitionInfoFilter
    {
        public Guid AccountBEDefinitionId { get; set; }
        public List<IFinancialAccountDefinitionFilter> Filters { get; set; }
    }
    public interface IFinancialAccountDefinitionFilter
    {
        bool IsMatched(IFinancialAccountDefinitionFilterContext context);
    }
    public interface IFinancialAccountDefinitionFilterContext
    {
        Guid FinancialAccountDefinitionId { get; }
        FinancialAccountDefinitionSettings DefinitionSettings { get;  }
    }
}
