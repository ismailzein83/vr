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
    public class SummaryTransformationDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_SummaryTransformationDefinition = "SummaryTransformationDefinition";

        ISummaryTransformationDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ISummaryTransformationDefinitionDataManager>();
        ISummaryTransformationDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ISummaryTransformationDefinitionDataManager>();
        [TestMethod]
        public void AddUpdateSelectSummaryTransformationDefinitions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_SummaryTransformationDefinition);
            SummaryTransformationDefinition SummaryTransformationDefinition = new SummaryTransformationDefinition
            {
                SummaryTransformationDefinitionId = Guid.NewGuid(),
                Name = "RDB TEST",
                DataRecordStorageId = Guid.NewGuid(),
                SummaryBatchStartFieldName = "RDB Tests",
                SummaryFromRawSettings = new UpdateSummaryFromRawSettings(),
                SummaryIdFieldName = "Tests",
                SummaryItemRecordTypeId = Guid.NewGuid(),
                UpdateExistingSummaryFromNewSettings = new UpdateExistingSummaryFromNewSettings(),
                RawItemRecordTypeId = Guid.NewGuid(),
                KeyFieldMappings = new List<SummaryTransformationKeyFieldMapping>(),
                RawTimeFieldName = "test",
            };
            var tester = new SummaryTransformationDefinitionMainOperationTester(SummaryTransformationDefinition, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class SummaryTransformationDefinitionMainOperationTester : EntityMainOperationTester<SummaryTransformationDefinition, ISummaryTransformationDefinitionDataManager>
        {
            public SummaryTransformationDefinitionMainOperationTester(SummaryTransformationDefinition SummaryTransformationDefinition, ISummaryTransformationDefinitionDataManager rdbDataManager, ISummaryTransformationDefinitionDataManager sqlDataManager)
                : base(SummaryTransformationDefinition, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<SummaryTransformationDefinition> context)
            {
                SummaryTransformationDefinition entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<SummaryTransformationDefinition> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<ISummaryTransformationDefinitionDataManager> context)
            {
                return context.DataManager.GetSummaryTransformationDefinitions();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<SummaryTransformationDefinition, ISummaryTransformationDefinitionDataManager> context)
            {
                return context.DataManager.AddSummaryTransformationDefinition(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<SummaryTransformationDefinition, ISummaryTransformationDefinitionDataManager> context)
            {
                return context.DataManager.UpdateSummaryTransformationDefinition(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_SummaryTransformationDefinition;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<SummaryTransformationDefinition> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<SummaryTransformationDefinition> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<SummaryTransformationDefinition, ISummaryTransformationDefinitionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<SummaryTransformationDefinition> context)
            {
                context.Entity.SummaryTransformationDefinitionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
