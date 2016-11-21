using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public interface IExistingEntity : Vanrise.Entities.IDateEffectiveSettings
    {
        IChangedEntity ChangedEntity { get; }
        DateTime? OriginalEED { get; }
        bool IsSameEntity(IExistingEntity nextEntity);
    }
}
