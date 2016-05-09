using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public interface ICPParametersContext
    {
        DateTime EffectiveDate { get; }

        int SellingNumberPlanId { get; }
    }
}
