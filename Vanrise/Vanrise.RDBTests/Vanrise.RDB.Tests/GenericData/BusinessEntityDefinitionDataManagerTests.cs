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
    public class BusinessEntityDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_BusinessEntityDefinition = "BusinessEntityDefinition";

        IBusinessEntityDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
        IBusinessEntityDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
        [TestMethod]
        public void AddUpdateSelectBusinessEntityDefinitions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_BusinessEntityDefinition);
            BusinessEntityDefinition BusinessEntityDefinition = new BusinessEntityDefinition
            {
                BusinessEntityDefinitionId = Guid.NewGuid(),
                Name = "RDB TEST",
                Settings = new StaticBEDefinitionSettings(),
                Title = "RDB TEST"
            };
            var tester = new BusinessEntityDefinitionMainOperationTester(BusinessEntityDefinition, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class BusinessEntityDefinitionMainOperationTester : EntityMainOperationTester<BusinessEntityDefinition, IBusinessEntityDefinitionDataManager>
        {
            public BusinessEntityDefinitionMainOperationTester(BusinessEntityDefinition BusinessEntityDefinition, IBusinessEntityDefinitionDataManager rdbDataManager, IBusinessEntityDefinitionDataManager sqlDataManager)
                : base(BusinessEntityDefinition, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<BusinessEntityDefinition> context)
            {
                BusinessEntityDefinition entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<BusinessEntityDefinition> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IBusinessEntityDefinitionDataManager> context)
            {
                return context.DataManager.GetBusinessEntityDefinitions();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<BusinessEntityDefinition, IBusinessEntityDefinitionDataManager> context)
            {
                return context.DataManager.AddBusinessEntityDefinition(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<BusinessEntityDefinition, IBusinessEntityDefinitionDataManager> context)
            {
                return context.DataManager.UpdateBusinessEntityDefinition(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_BusinessEntityDefinition;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<BusinessEntityDefinition> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<BusinessEntityDefinition> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<BusinessEntityDefinition, IBusinessEntityDefinitionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<BusinessEntityDefinition> context)
            {
                context.Entity.BusinessEntityDefinitionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
