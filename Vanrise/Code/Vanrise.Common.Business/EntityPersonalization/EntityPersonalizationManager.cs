using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class EntityPersonalizationManager
    {
        public EntityPersonalization GetCurrentUserEntityPersonalization(string entityUniqueKey)
        {
            throw new NotImplementedException();
        }
        public EntityPersonalization GetGlobalEntityPersonalization(string entityUniqueKey)
        {
            throw new NotImplementedException();
        }

        public void UpdateCurrentUserEntityPersonalization(string entityUniqueKey, EntityPersonalization entityPersonalization)
        {
            throw new NotImplementedException();
        }

        public void UpdateGlobalEntityPersonalization(string entityUniqueKey, EntityPersonalization entityPersonalization)
        {
            throw new NotImplementedException();
        }

        public void DeleteCurrentUserEntityPersonalization(string entityUniqueKey)
        {
            throw new NotImplementedException();
        }

        public void DeleteGlobalEntityPersonalization(string entityUniqueKey)
        {
            throw new NotImplementedException();
        }
    }
}
