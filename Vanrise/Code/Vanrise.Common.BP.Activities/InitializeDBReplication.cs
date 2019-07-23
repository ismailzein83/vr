using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.BP.Activities
{

    public class InitializeDBReplicationInput
    {
        public DBReplicationSettings DBReplicationSettings { get; set; }

        public DBReplicationDefinition DBReplicatioDefinition { get; set; }
    }

    public class InitializeDBReplicationOutput
    {
        public IDBReplicationDataManager DBReplicationDataManager { get; set; }
    }

    public sealed class InitializeDBReplication : BaseAsyncActivity<InitializeDBReplicationInput, InitializeDBReplicationOutput>
    {
        [RequiredArgument]
        public InArgument<DBReplicationSettings> DBReplicationSettings { get; set; }

        [RequiredArgument]
        public InArgument<DBReplicationDefinition> DBReplicatioDefinition { get; set; }

        [RequiredArgument]
        public OutArgument<IDBReplicationDataManager> DBReplicationDataManager { get; set; }

        protected override InitializeDBReplicationOutput DoWorkWithResult(InitializeDBReplicationInput inputArgument, AsyncActivityHandle handle)
        {
            DBReplicationSettings dbReplicationSettings = inputArgument.DBReplicationSettings;
            DBReplicationDefinition dbReplicatioDefinition = inputArgument.DBReplicatioDefinition;

            IDBReplicationDataManager dbReplicationDataManager = CommonDataManagerFactory.GetDataManager<IDBReplicationDataManager>();
            VRConnectionManager vrConnectionManager = new VRConnectionManager();

            Dictionary<string, List<DBReplicationTableDetails>> dbReplicationTableDetailsListByTargetLinkedServer = new Dictionary<string, List<DBReplicationTableDetails>>();
            Dictionary<Guid, DBConnectionEntity> dbConnectionEntities = GetDBConnectionEntities(dbReplicationSettings);

            foreach (var databaseDefinitionKvp in dbReplicatioDefinition.Settings.DatabaseDefinitions)
            {
                if (ShouldStop(handle))
                    break;

                Guid databaseDefinitionId = databaseDefinitionKvp.Key;

                DBConnectionEntity dbConnectionEntity = dbConnectionEntities.GetRecord(databaseDefinitionId);
                VRConnection vrConnection = vrConnectionManager.GetVRConnection(dbConnectionEntity.TargetConnectionId);
                SQLConnection sqlConnection = vrConnection.Settings.CastWithValidate<SQLConnection>("SQLConnection", SQLConnection.s_ConfigId);

                foreach (DBReplicationTableDefinition table in databaseDefinitionKvp.Value.Tables)
                {
                    if (ShouldStop(handle))
                        break;

                    string connectionString;

                    if (sqlConnection.ConnectionString != null)
                        connectionString = sqlConnection.ConnectionString;
                    else if (sqlConnection.ConnectionStringAppSettingName != null)
                        connectionString = sqlConnection.ConnectionStringAppSettingName;
                    else
                        connectionString = sqlConnection.ConnectionStringName;

                    DBReplicationTableDetails dbReplicationTableDetails = new DBReplicationTableDetails()
                    {
                        DBReplicationPreInsert = table.DBReplicationPreInsert,
                        FilterDateTimeColumn = table.FilterDateTimeColumn,
                        IdColumn = table.IdColumn,
                        ChunkSize = table.ChunkSize,
                        TableSchema = table.TableSchema,
                        SourceConnectionStringName = dbConnectionEntity.SourceConnectionStringName,
                        TableName = table.TableName,
                        TargetConnectionString = connectionString
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
                    handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
                },
                ShouldStop = () => { return ShouldStop(handle); }
            });

            return new InitializeDBReplicationOutput
            {
                DBReplicationDataManager = dbReplicationDataManager
            };
        }

        protected override InitializeDBReplicationInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new InitializeDBReplicationInput()
            {
                DBReplicatioDefinition = this.DBReplicatioDefinition.Get(context),
                DBReplicationSettings = this.DBReplicationSettings.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, InitializeDBReplicationOutput result)
        {
            context.SetValue(this.DBReplicationDataManager, result.DBReplicationDataManager);
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