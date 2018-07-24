using System;
using System.Collections.Generic;
using TestRuntime;
using Vanrise.Entities;

namespace TOne.WhS.Runtime.Tasks
{
    public class FatimaAlheckTask : ITask
    {
        public void Execute()
        {
            var dbReplicationDatabaseDefinitions = new Dictionary<Guid, DBReplicationDatabaseDefinition>();
            dbReplicationDatabaseDefinitions.Add(Guid.NewGuid(), new DBReplicationDatabaseDefinition()
            {
                Name = "TOneConfiguration",
                Tables = new List<DBReplicationTableDefinition>()
                {
                    new DBReplicationTableDefinition(){TableName = "StatusDefinition", TableSchema = "Common"},
                    new DBReplicationTableDefinition(){TableName = "VRAppVisibility", TableSchema = "Common"},
                    new DBReplicationTableDefinition(){TableName = "BPDefinition", TableSchema = "bp"}
                }
            });
            dbReplicationDatabaseDefinitions.Add(Guid.NewGuid(), new DBReplicationDatabaseDefinition()
            {
                Name = "TOneV2_Dev",
                Tables = new List<DBReplicationTableDefinition>()
                {
                    new DBReplicationTableDefinition(){TableName = "Currency", TableSchema = "Common"},
                    new DBReplicationTableDefinition(){TableName = "AccountManager", TableSchema = "TOneWhS_BE"},
                    new DBReplicationTableDefinition(){TableName = "ANumberGroup", TableSchema = "TOneWhS_BE"}
                }
            });

            //Name = "DBReplicationDefinition",
            //VRComponentTypeId = Guid.NewGuid(),
            DBReplicationDefinitionSettings dbReplicationDefinitionSettings = new DBReplicationDefinitionSettings()
            {
                DatabaseDefinitions = dbReplicationDatabaseDefinitions
            };

            string serializedDBReplicationDefinition = Vanrise.Common.Serializer.Serialize(dbReplicationDefinitionSettings);
        }
    }
}