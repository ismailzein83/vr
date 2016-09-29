using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface IExistingEntity : Vanrise.Entities.IDateEffectiveSettings
    {
        IChangedEntity ChangedEntity { get; }

        bool IsSameEntity(IExistingEntity nextEntity);
    }
}
