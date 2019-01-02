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
    public class OverriddenConfigurationDataManagerTests
    {
        const string DBTABLE_NAME_OverriddenConfiguration = "OverriddenConfiguration";

        IOverriddenConfigurationDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IOverriddenConfigurationDataManager>();
        IOverriddenConfigurationDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IOverriddenConfigurationDataManager>();
        [TestMethod]
        public void AddUpdateSelectOverriddenConfigurations()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_OverriddenConfiguration);
            OverriddenConfiguration OverriddenConfiguration = new OverriddenConfiguration
            {
                OverriddenConfigurationId = Guid.NewGuid(),
                Name = "RDB TEST",
                Settings = new OverriddenConfigurationSettings
                {
                },
                GroupId = Guid.NewGuid()
            };
            var tester = new OverriddenConfigurationMainOperationTester(OverriddenConfiguration, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class OverriddenConfigurationMainOperationTester : EntityMainOperationTester<OverriddenConfiguration, IOverriddenConfigurationDataManager>
        {
            public OverriddenConfigurationMainOperationTester(OverriddenConfiguration OverriddenConfiguration, IOverriddenConfigurationDataManager rdbDataManager, IOverriddenConfigurationDataManager sqlDataManager)
                : base(OverriddenConfiguration, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<OverriddenConfiguration> context)
            {
                OverriddenConfiguration entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<OverriddenConfiguration> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IOverriddenConfigurationDataManager> context)
            {
                return context.DataManager.GetOverriddenConfigurations();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<OverriddenConfiguration, IOverriddenConfigurationDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<OverriddenConfiguration, IOverriddenConfigurationDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_OverriddenConfiguration;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<OverriddenConfiguration> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<OverriddenConfiguration> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<OverriddenConfiguration, IOverriddenConfigurationDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<OverriddenConfiguration> context)
            {
                context.Entity.OverriddenConfigurationId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
