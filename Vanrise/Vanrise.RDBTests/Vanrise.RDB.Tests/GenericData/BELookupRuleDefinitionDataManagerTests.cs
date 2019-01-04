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
    public class BELookupRuleDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_BELookupRuleDefinition = "BELookupRuleDefinition";

        IBELookupRuleDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBELookupRuleDefinitionDataManager>();
        IBELookupRuleDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBELookupRuleDefinitionDataManager>();
        [TestMethod]
        public void AddUpdateSelectBELookupRuleDefinitions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_BELookupRuleDefinition);
            BELookupRuleDefinition BELookupRuleDefinition = new BELookupRuleDefinition
            {
                BELookupRuleDefinitionId = Guid.NewGuid(),
                Name = "RDB TEST",
                BusinessEntityDefinitionId = Guid.NewGuid(),
                CriteriaFields = new List<BELookupRuleDefinitionCriteriaField>(),
            };
            var tester = new BELookupRuleDefinitionMainOperationTester(BELookupRuleDefinition, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class BELookupRuleDefinitionMainOperationTester : EntityMainOperationTester<BELookupRuleDefinition, IBELookupRuleDefinitionDataManager>
        {
            public BELookupRuleDefinitionMainOperationTester(BELookupRuleDefinition BELookupRuleDefinition, IBELookupRuleDefinitionDataManager rdbDataManager, IBELookupRuleDefinitionDataManager sqlDataManager)
                : base(BELookupRuleDefinition, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<BELookupRuleDefinition> context)
            {
                BELookupRuleDefinition entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<BELookupRuleDefinition> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IBELookupRuleDefinitionDataManager> context)
            {
                return context.DataManager.GetBELookupRuleDefinitions();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<BELookupRuleDefinition, IBELookupRuleDefinitionDataManager> context)
            {
                return context.DataManager.InsertBELookupRuleDefinition(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<BELookupRuleDefinition, IBELookupRuleDefinitionDataManager> context)
            {
                return context.DataManager.UpdateBELookupRuleDefinition(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_BELookupRuleDefinition;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<BELookupRuleDefinition> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<BELookupRuleDefinition> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<BELookupRuleDefinition, IBELookupRuleDefinitionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<BELookupRuleDefinition> context)
            {
                context.Entity.BELookupRuleDefinitionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
