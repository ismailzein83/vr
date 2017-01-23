using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IAccountTypeFilter
    {
        bool IsMatched(IAccountTypeFilterContext context);
    }

    public interface IAccountTypeFilterContext
    {
        AccountType AccountType { get; }
    }
}
