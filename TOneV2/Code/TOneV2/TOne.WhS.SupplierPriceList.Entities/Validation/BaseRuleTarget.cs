using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public abstract class BaseRuleTarget<T>
    {
        public abstract bool isValid();

        public abstract void SetExecluded(List<T> list);

    }
}
