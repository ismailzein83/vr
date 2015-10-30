using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public interface IRuleInCarrierCriteria
    {
        List<string> IN_Carriers { get; }
    }
}
