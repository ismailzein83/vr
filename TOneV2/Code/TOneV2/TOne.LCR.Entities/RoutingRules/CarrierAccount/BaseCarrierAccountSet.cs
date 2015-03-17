using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public abstract class BaseCarrierAccountSet
    {
        public abstract string Description { get; }
        public virtual bool IsAccountIdIncluded(string accountId)
        {
            return true;
        }
    }
}
