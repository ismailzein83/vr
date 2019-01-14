using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Data;
using Vanrise.RDBTests.Common;
using Vanrise.GenericData.Transformation.Entities;
namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class DataTransformationDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_DataTransformationDefinition = "DataTransformationDefinition";

        IDataTransformationDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IDataTransformationDefinitionDataManager>();
        IDataTransformationDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IDataTransformationDefinitionDataManager>();
        [TestMethod]
        public void AddUpdateSelectDataTransformationDefinitions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_DataTransformationDefinition);
            DataTransformationDefinition DataTransformationDefinition = new DataTransformationDefinition
            {
                DataTransformationDefinitionId = Guid.NewGuid(),
                Name = "RDB TEST",
               MappingSteps = new List<MappingStep>(),
               RecordTypes = new List<DataTransformationRecordType>(),
               Title ="test"
            };
            var tester = new DataTransformationDefinitionMainOperationTester(DataTransformationDefinition, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class DataTransformationDefinitionMainOperationTester : EntityMainOperationTester<DataTransformationDefinition, IDataTransformationDefinitionDataManager>
        {
            public DataTransformationDefinitionMainOperationTester(DataTransformationDefinition DataTransformationDefinition, IDataTransformationDefinitionDataManager rdbDataManager, IDataTransformationDefinitionDataManager sqlDataManager)
                : base(DataTransformationDefinition, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<DataTransformationDefinition> context)
            {
                DataTransformationDefinition entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<DataTransformationDefinition> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IDataTransformationDefinitionDataManager> context)
            {
                return context.DataManager.GetDataTransformationDefinitions();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<DataTransformationDefinition, IDataTransformationDefinitionDataManager> context)
            {
                return context.DataManager.AddDataTransformationDefinition(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<DataTransformationDefinition, IDataTransformationDefinitionDataManager> context)
            {
                return context.DataManager.UpdateDataTransformationDefinition(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_DataTransformationDefinition;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<DataTransformationDefinition> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<DataTransformationDefinition> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<DataTransformationDefinition, IDataTransformationDefinitionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<DataTransformationDefinition> context)
            {
                context.Entity.DataTransformationDefinitionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
