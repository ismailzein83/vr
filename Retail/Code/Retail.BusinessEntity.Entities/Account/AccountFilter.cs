using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountFilter
    {
        public IEnumerable<IAccountFilter> Filters { get; set; }
    }

    public interface  IAccountFilter
    {
        bool IsExcluded(IAccountFilterContext context);
    }

    public interface IAccountFilterContext
    {
        Account Account { get; }
    }

    public class AccountFilterContext : IAccountFilterContext
    {
        public Account Account { get; set; }
    }
}
