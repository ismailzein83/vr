using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.RDBTests.Common;
using Vanrise.Entities;
namespace Vanrise.RDB.Tests.Common
{
    [TestClass]
    public class StyleDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_StyleDefinition = "StyleDefinition";

        IStyleDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IStyleDefinitionDataManager>();
        IStyleDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IStyleDefinitionDataManager>();

        [TestMethod]
        public void AddUpdateSelectStyleDefinitions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_StyleDefinition);
            StyleDefinition StyleDefinition = new StyleDefinition
            {
               Name = "RDB TEST",
               StyleDefinitionId = Guid.NewGuid(),
               StyleDefinitionSettings = new StyleDefinitionSettings
               {
               }
            };
            var tester = new StyleDefinitionMainOperationTester(StyleDefinition, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class StyleDefinitionMainOperationTester : EntityMainOperationTester<StyleDefinition, IStyleDefinitionDataManager>
        {
            public StyleDefinitionMainOperationTester(StyleDefinition StyleDefinition, IStyleDefinitionDataManager rdbDataManager, IStyleDefinitionDataManager sqlDataManager)
                : base(StyleDefinition, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<StyleDefinition> context)
            {
                StyleDefinition entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.StyleDefinitionId = Guid.NewGuid();
                entity.StyleDefinitionSettings = new StyleDefinitionSettings();
            }
            public override IEnumerable<StyleDefinition> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IStyleDefinitionDataManager> context)
            {
                var styleDefinitions = context.DataManager.GetStyleDefinitions();
                if (styleDefinitions == null)
                    return null;
                foreach (var StyleDefinition in styleDefinitions)
                {
                }
                return styleDefinitions;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<StyleDefinition, IStyleDefinitionDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<StyleDefinition, IStyleDefinitionDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_StyleDefinition;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<StyleDefinition> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<StyleDefinition> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<StyleDefinition, IStyleDefinitionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<StyleDefinition> context)
            {
                context.Entity.StyleDefinitionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
