using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.BP.Activities
{
    public sealed class InitializeDBReplication : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<DBReplicationSettings> DBReplicationSettings { get; set; }

        [RequiredArgument]
        public InArgument<DBReplicationDefinition> DBReplicatioDefinition { get; set; }

        [RequiredArgument]
        public OutArgument<IDBReplicationDataManager> DBReplicationDataManager { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            DBReplicationSettings dbReplicationSettings = this.DBReplicationSettings.Get(context.ActivityContext);
            DBReplicationDefinition dbReplicatioDefinition = this.DBReplicatioDefinition.Get(context.ActivityContext);

            IDBReplicationDataManager dbReplicationDataManager = CommonDataManagerFactory.GetDataManager<IDBReplicationDataManager>();
            VRConnectionManager vrConnectionManager = new VRConnectionManager();

            Dictionary<string, List<DBReplicationTableDetails>> dbReplicationTableDetailsListByTargetLinkedServer = new Dictionary<string, List<DBReplicationTableDetails>>();
            Dictionary<Guid, DBConnectionEntity> dbConnectionEntities = GetDBConnectionEntities(dbReplicationSettings);

            foreach (var databaseDefinitionKvp in dbReplicatioDefinition.Settings.DatabaseDefinitions)
            {
                Guid databaseDefinitionId = databaseDefinitionKvp.Key;

                DBConnectionEntity dbConnectionEntity = dbConnectionEntities.GetRecord(databaseDefinitionId);
                VRConnection vrConnection = vrConnectionManager.GetVRConnection(dbConnectionEntity.TargetConnectionId);
                SQLConnection sqlConnection = vrConnection.Settings.CastWithValidate<SQLConnection>("SQLConnection", SQLConnection.s_ConfigId);

                foreach (DBReplicationTableDefinition table in databaseDefinitionKvp.Value.Tables)
                {
                    DBReplicationTableDetails dbReplicationTableDetails = new DBReplicationTableDetails()
                    {
                        DBReplicationPreInsert = table.DBReplicationPreInsert,
                        FilterDateTimeColumn = table.FilterDateTimeColumn,
                        IdColumn = table.IdColumn,
                        ChunkSize = table.ChunkSize,
                        TableSchema = table.TableSchema,
                        SourceConnectionStringName = dbConnectionEntity.SourceConnectionStringName,
                        TableName = table.TableName,
                        TargetConnectionString = sqlConnection.ConnectionString
                    };

                    List<DBReplicationTableDetails> dbReplicationTableDetailsList = dbReplicationTableDetailsListByTargetLinkedServer.GetOrCreateItem(dbReplicationTableDetails.TargetConnectionString);
                    dbReplicationTableDetailsList.Add(dbReplicationTableDetails);
                }
            }

            dbReplicationDataManager.Initialise(new DBReplicationInitializeContext()
            {
                DBReplicationTableDetailsListByTargetServer = dbReplicationTableDetailsListByTargetLinkedServer,
                WriteInformation = (message) =>
                {
                    context.ActivityContext.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
                }
            });

            this.DBReplicationDataManager.Set(context.ActivityContext, dbReplicationDataManager);
        }

        private Dictionary<Guid, DBConnectionEntity> GetDBConnectionEntities(DBReplicationSettings dbReplicationSettings)
        {
            if (dbReplicationSettings.DBConnections == null)
                return null;

            Dictionary<Guid, DBConnectionEntity> result = new Dictionary<Guid, DBConnectionEntity>();
            foreach (var dbConnection in dbReplicationSettings.DBConnections)
            {
                DBConnectionEntity dbConnectionEntity = new DBConnectionEntity()
                {
                    SourceConnectionStringName = dbConnection.SourceConnectionStringName,
                    TargetConnectionId = dbConnection.TargetConnectionId
                };

                foreach (var setting in dbConnection.Settings)
                    result.Add(setting.DatabaseDefinitionId, dbConnectionEntity);
            }
            return result;
        }

        private class DBConnectionEntity
        {
            public string SourceConnectionStringName { get; set; }

            public Guid TargetConnectionId { get; set; }
        }
    }
}