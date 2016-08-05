using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Data.SQL
{
    public class RouteSyncDefinitionDataManager : BaseSQLDataManager, IRouteSyncDefinitionDataManager
    {
        public RouteSyncDefinitionDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        { }

        public List<RouteSyncDefinition> GetRouteSyncDefinitions()
        {
            return GetItemsSP("[TOneWhS_RouteSync].[sp_RouteSyncDefinition_GetAll]", RouteSyncDefinitionMapper);
        }

        private RouteSyncDefinition RouteSyncDefinitionMapper(IDataReader reader)
        {
            return new RouteSyncDefinition()
            {
                RouteSyncDefinitionId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Serializer.Deserialize<RouteSyncDefinitionSettings>(reader["Settings"] as string)
            };
        }

        public bool AreRouteSyncDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_RouteSync.RouteSyncDefinition", ref updateHandle);
        }
    }
}
