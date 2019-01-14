using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.RDBTests.Common;
using Vanrise.GenericData.Entities;
namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class DataRecordStorageDataManagerTests
    {
        const string DBTABLE_NAME_DataRecordStorage = "DataRecordStorage";

        IDataRecordStorageDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();
        IDataRecordStorageDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();
        [TestMethod]
        public void AddUpdateSelectDataRecordStorages()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_DataRecordStorage);
            DataRecordStorage DataRecordStorage = new DataRecordStorage
            {
                DataRecordStorageId = Guid.NewGuid(),
                Name = "RDB TEST",
                Settings = new DataRecordStorageSettings(),
                DataRecordTypeId = Guid.NewGuid(),
                DataStoreId = Guid.NewGuid(),
                State = new DataRecordStorageState()
            };
            var tester = new DataRecordStorageMainOperationTester(DataRecordStorage, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class DataRecordStorageMainOperationTester : EntityMainOperationTester<DataRecordStorage, IDataRecordStorageDataManager>
        {
            public DataRecordStorageMainOperationTester(DataRecordStorage DataRecordStorage, IDataRecordStorageDataManager rdbDataManager, IDataRecordStorageDataManager sqlDataManager)
                : base(DataRecordStorage, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<DataRecordStorage> context)
            {
                DataRecordStorage entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<DataRecordStorage> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IDataRecordStorageDataManager> context)
            {
                return context.DataManager.GetDataRecordStorages();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<DataRecordStorage, IDataRecordStorageDataManager> context)
            {
                return context.DataManager.AddDataRecordStorage(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<DataRecordStorage, IDataRecordStorageDataManager> context)
            {
                return context.DataManager.UpdateDataRecordStorage(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_DataRecordStorage;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<DataRecordStorage> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<DataRecordStorage> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<DataRecordStorage, IDataRecordStorageDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<DataRecordStorage> context)
            {
                context.Entity.DataRecordStorageId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
