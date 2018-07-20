using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class DBReplicationDefinitionManager
    {
        #region Public Methods
        public DBReplicationDefinition GetDBReplicationDefinition(Guid dbReplicationDefinitionId)
        {
            var dbReplicationDefinitions = this.GetCachedDBReplicationDefinitions();
            return dbReplicationDefinitions.FindRecord(x => x.VRComponentTypeId == dbReplicationDefinitionId);
        }

        public IEnumerable<DBReplicationDefinitionInfo> GetDBReplicationDefinitionsInfo(DBReplicationDefinitionInfoFilter filter)
        {
            Func<DBReplicationDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (item) =>
                {
                    return true;
                };
            }

            return GetCachedDBReplicationDefinitions().MapRecords(DBReplicationDefinitionInfoMapper, filterExpression);
        }

        public IEnumerable<DBDefinitionInfo> GetDBDefinitionsInfo(Guid dbReplicationDefinitionId, DBDefinitionInfoFilter filter)
        {
            var dbReplicationDefinition = GetDBReplicationDefinition(dbReplicationDefinitionId);
            var dbDefinitions = dbReplicationDefinition.Settings.CastWithValidate<DBReplicationDefinitionSettings>("DBReplicationDefinitionSettings").DatabaseDefinitions;

            List<DBDefinitionInfo> dbDefinitionsInfo = new List<DBDefinitionInfo>();

            foreach (var databaseDefinition in dbDefinitions)
            {
                dbDefinitionsInfo.Add(new DBDefinitionInfo
                {
                    DBDefinitionId = databaseDefinition.Key,
                    Name = databaseDefinition.Value.Name
                });
            }

            Func<DBDefinitionInfo, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (item) =>
                {
                    if (filter.ForceDBDefinitionIds != null && filter.ForceDBDefinitionIds.Contains(item.DBDefinitionId))
                        return true;

                    if (filter.ExcludedDBDefinitionIds != null && filter.ExcludedDBDefinitionIds.Contains(item.DBDefinitionId))
                        return false;

                    return true;
                };
            }
            return dbDefinitionsInfo.MapRecords(record => record, filterExpression);
        }

        #endregion

        #region Private Methods
        private Dictionary<Guid, DBReplicationDefinition> GetCachedDBReplicationDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<DBReplicationDefinitionSettings, DBReplicationDefinition>();
        }

        #endregion

        #region Mappers

        private DBReplicationDefinitionInfo DBReplicationDefinitionInfoMapper(DBReplicationDefinition dbReplicationDefinition)
        {
            return new DBReplicationDefinitionInfo
            {
                DBReplicationDefinitionId = dbReplicationDefinition.VRComponentTypeId,
                Name = dbReplicationDefinition.Name,
            };
        }

        #endregion
    }
}
