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
    public class StatusDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_StatusDefinition = "StatusDefinition";

        IStatusDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
        IStatusDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();

        [TestMethod]
        public void AddUpdateSelectStatusDefinitions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_StatusDefinition);
            StatusDefinition StatusDefinition = new StatusDefinition
            {
                Name = "RDB TEST",
                BusinessEntityDefinitionId = Guid.NewGuid(),
                Settings = new StatusDefinitionSettings
                {
                    HasInitialCharge = false,
                    HasRecurringCharge = false,
                    IsActive = true,
                },
                StatusDefinitionId = Guid.NewGuid(),
                CreatedBy = 1,
                LastModifiedBy = 1
            };
            var tester = new StatusDefinitionMainOperationTester(StatusDefinition, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class StatusDefinitionMainOperationTester : EntityMainOperationTester<StatusDefinition, IStatusDefinitionDataManager>
        {
            public StatusDefinitionMainOperationTester(StatusDefinition StatusDefinition, IStatusDefinitionDataManager rdbDataManager, IStatusDefinitionDataManager sqlDataManager)
                : base(StatusDefinition, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<StatusDefinition> context)
            {
                StatusDefinition entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.Settings.IsInvoiceActive = true;
                entity.Settings.IsAccountBalanceActive = true;
            }
            public override IEnumerable<StatusDefinition> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IStatusDefinitionDataManager> context)
            {
                var statusDefinitions = context.DataManager.GetStatusDefinition();
                if (statusDefinitions == null)
                    return null;
                foreach (var statusDefinition in statusDefinitions)
                {
                    statusDefinition.CreatedTime = statusDefinition.CreatedTime.Date;
                    if (statusDefinition.LastModifiedTime.HasValue)
                        statusDefinition.LastModifiedTime = statusDefinition.LastModifiedTime.Value.Date;
                }
                return statusDefinitions;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<StatusDefinition, IStatusDefinitionDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<StatusDefinition, IStatusDefinitionDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_StatusDefinition;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<StatusDefinition> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<StatusDefinition> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<StatusDefinition, IStatusDefinitionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<StatusDefinition> context)
            {
                context.Entity.StatusDefinitionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
