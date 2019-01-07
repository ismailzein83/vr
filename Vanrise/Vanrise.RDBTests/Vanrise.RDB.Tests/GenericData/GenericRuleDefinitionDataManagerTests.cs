using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.RDBTests.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Normalization;

namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class GenericRuleDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_GenericRuleDefinition = "GenericRuleDefinition";

        IGenericRuleDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
        IGenericRuleDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
        [TestMethod]
        public void AddUpdateSelectGenericRuleDefinitions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_GenericRuleDefinition);
            GenericRuleDefinition GenericRuleDefinition = new GenericRuleDefinition
            {
                GenericRuleDefinitionId = Guid.NewGuid(),
                Name = "RDB TEST",
                Objects = new Entities.VRObjectVariableCollection (),
                Title = "RDB TEST",
                SettingsDefinition =new NormalizationRuleDefinitionSettings(),
                CriteriaDefinition =new CompositeRecordConditionDefinitionGroup()
            };
            var tester = new GenericRuleDefinitionMainOperationTester(GenericRuleDefinition, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class GenericRuleDefinitionMainOperationTester : EntityMainOperationTester<GenericRuleDefinition, IGenericRuleDefinitionDataManager>
        {
            public GenericRuleDefinitionMainOperationTester(GenericRuleDefinition GenericRuleDefinition, IGenericRuleDefinitionDataManager rdbDataManager, IGenericRuleDefinitionDataManager sqlDataManager)
                : base(GenericRuleDefinition, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<GenericRuleDefinition> context)
            {
                GenericRuleDefinition entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<GenericRuleDefinition> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IGenericRuleDefinitionDataManager> context)
            {
                return context.DataManager.GetGenericRuleDefinitions();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<GenericRuleDefinition, IGenericRuleDefinitionDataManager> context)
            {
                return context.DataManager.AddGenericRuleDefinition(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<GenericRuleDefinition, IGenericRuleDefinitionDataManager> context)
            {
                return context.DataManager.UpdateGenericRuleDefinition(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_GenericRuleDefinition;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<GenericRuleDefinition> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<GenericRuleDefinition> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<GenericRuleDefinition, IGenericRuleDefinitionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<GenericRuleDefinition> context)
            {
                context.Entity.GenericRuleDefinitionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
