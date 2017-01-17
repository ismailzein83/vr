using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IDIDFilter
    {
        bool IsMatched(IDIDFilterContext context);
    }

    public interface IDIDFilterContext
    {
        DID DID { get; }
    }
}
