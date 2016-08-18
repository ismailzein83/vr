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
        #region ctor/Local Variables
        public RouteSyncDefinitionDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        { }
        #endregion


        #region Public Methods
        public List<RouteSyncDefinition> GetRouteSyncDefinitions()
        {
            return GetItemsSP("[TOneWhS_RouteSync].[sp_RouteSyncDefinition_GetAll]", RouteSyncDefinitionMapper);
        }

        public bool AreRouteSyncDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_RouteSync.RouteSyncDefinition", ref updateHandle);
        }

        public bool Insert(RouteSyncDefinition routeSyncDefinitionItem, out int insertedId)
        {
            object reprocessDefinitionID;
            string serializedSettings = routeSyncDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(routeSyncDefinitionItem.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("TOneWhS_RouteSync.sp_RouteSyncDefinition_Insert", out reprocessDefinitionID, routeSyncDefinitionItem.Name, serializedSettings);

            insertedId = (affectedRecords > 0) ? (int)reprocessDefinitionID : -1;
            return (affectedRecords > 0);
        }

        public bool Update(RouteSyncDefinition routeSyncDefinitionItem)
        {
            string serializedSettings = routeSyncDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(routeSyncDefinitionItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("TOneWhS_RouteSync.sp_RouteSyncDefinition_Update", routeSyncDefinitionItem.RouteSyncDefinitionId, routeSyncDefinitionItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }
        #endregion


        #region Mappers
        private RouteSyncDefinition RouteSyncDefinitionMapper(IDataReader reader)
        {
            return new RouteSyncDefinition()
            {
                RouteSyncDefinitionId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Serializer.Deserialize<RouteSyncDefinitionSettings>(reader["Settings"] as string)
            };
        }
        #endregion
    }
}
