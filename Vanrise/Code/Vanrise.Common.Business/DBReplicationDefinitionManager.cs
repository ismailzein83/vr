using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class DBReplicationDefinitionManager
    {
        public DBReplicationDefinition GetDBReplicationDefinition(Guid dbReplicationDefinitionId)
        {
            var dbReplicationDefinitions = this.GetCachedDBReplicationDefinitions();
            return dbReplicationDefinitions.FindRecord(x => x.VRComponentTypeId == dbReplicationDefinitionId);
        }

        private Dictionary<Guid, DBReplicationDefinition> GetCachedDBReplicationDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<DBReplicationDefinitionSettings, DBReplicationDefinition>();
        }
    }
}
