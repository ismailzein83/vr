using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IEntityPersonalizationDataManager : IDataManager
    {
        List<EntityPersonalization> GetEntityPersonalizations();

        bool AreEntityPersonalizationUpdated(ref object updateHandle);

        bool Save(EntityPersonalization entityPersonalization);

        bool Delete(long entityPersonalizationId);
    }
}
