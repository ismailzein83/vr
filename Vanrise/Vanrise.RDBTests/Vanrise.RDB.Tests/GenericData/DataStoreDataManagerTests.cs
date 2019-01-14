using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data.SQL;
using Vanrise.RDBTests.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.SQLDataStorage;

namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class DataStoreDataManagerTests
    {
        const string DBTABLE_NAME_DataStore = "DataStore";

        IDataStoreDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IDataStoreDataManager>();
        IDataStoreDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IDataStoreDataManager>();
        [TestMethod]
        public void AddUpdateSelectDataStores()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_DataStore);
            DataStore DataStore = new DataStore
            {
                DataStoreId = Guid.NewGuid(),
                Name = "RDB TEST",
                 Settings = new SQLDataStoreSettings()

            };
            var tester = new DataStoreMainOperationTester(DataStore, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class DataStoreMainOperationTester : EntityMainOperationTester<DataStore, IDataStoreDataManager>
        {
            public DataStoreMainOperationTester(DataStore DataStore, IDataStoreDataManager rdbDataManager, IDataStoreDataManager sqlDataManager)
                : base(DataStore, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<DataStore> context)
            {
                DataStore entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<DataStore> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IDataStoreDataManager> context)
            {
                return context.DataManager.GetDataStores();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<DataStore, IDataStoreDataManager> context)
            {
                return context.DataManager.AddDataStore(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<DataStore, IDataStoreDataManager> context)
            {
                return context.DataManager.UpdateDataStore(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_DataStore;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<DataStore> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<DataStore> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<DataStore, IDataStoreDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<DataStore> context)
            {
                context.Entity.DataStoreId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
