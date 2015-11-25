using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public enum ChangesApplyType { SaveDraft = 0, ApplyNow = 1 }

    public abstract class BaseChanges
    {
        public ChangesApplyType ApplyType { get; set; }
    }
}
