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
    public class DataRecordTypeDataManagerTests
    {
        const string DBTABLE_NAME_DataRecordType = "DataRecordType";

        IDataRecordTypeDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
        IDataRecordTypeDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
        [TestMethod]
        public void AddUpdateSelectDataRecordTypes()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_DataRecordType);
            DataRecordType DataRecordType = new DataRecordType
            {
                DataRecordTypeId = Guid.NewGuid(),
                Name = "RDB TEST",
                Settings = new DataRecordTypeSettings(),
               Fields = new List<DataRecordField>(),         
            };
            var tester = new DataRecordTypeMainOperationTester(DataRecordType, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
            _rdbDataManager.SetDataRecordTypeCacheExpired();
            _sqlDataManager.SetDataRecordTypeCacheExpired();
        }

        #region Private Classes

        private class DataRecordTypeMainOperationTester : EntityMainOperationTester<DataRecordType, IDataRecordTypeDataManager>
        {
            public DataRecordTypeMainOperationTester(DataRecordType DataRecordType, IDataRecordTypeDataManager rdbDataManager, IDataRecordTypeDataManager sqlDataManager)
                : base(DataRecordType, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<DataRecordType> context)
            {
                DataRecordType entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<DataRecordType> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IDataRecordTypeDataManager> context)
            {
                return context.DataManager.GetDataRecordTypes();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<DataRecordType, IDataRecordTypeDataManager> context)
            {
                return context.DataManager.AddDataRecordType(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<DataRecordType, IDataRecordTypeDataManager> context)
            {
                return context.DataManager.UpdateDataRecordType(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_DataRecordType;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<DataRecordType> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<DataRecordType> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<DataRecordType, IDataRecordTypeDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<DataRecordType> context)
            {
                context.Entity.DataRecordTypeId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
