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
    public class OverriddenConfigurationGroupGroupDataManagerTests
    {
        const string DBTABLE_NAME_OverriddenConfigurationGroup = "OverriddenConfigurationGroup";

        IOverriddenConfigurationGroupDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IOverriddenConfigurationGroupDataManager>();
        IOverriddenConfigurationGroupDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IOverriddenConfigurationGroupDataManager>();

        [TestMethod]
        public void AddUpdateSelectOverriddenConfigurationGroups()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_OverriddenConfigurationGroup);
            OverriddenConfigurationGroup OverriddenConfigurationGroup = new OverriddenConfigurationGroup
            {
                OverriddenConfigurationGroupId = Guid.NewGuid(),
                Name = "RDB TEST",
            };
            var tester = new OverriddenConfigurationGroupMainOperationTester(OverriddenConfigurationGroup, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class OverriddenConfigurationGroupMainOperationTester : EntityMainOperationTester<OverriddenConfigurationGroup, IOverriddenConfigurationGroupDataManager>
        {
            public OverriddenConfigurationGroupMainOperationTester(OverriddenConfigurationGroup OverriddenConfigurationGroup, IOverriddenConfigurationGroupDataManager rdbDataManager, IOverriddenConfigurationGroupDataManager sqlDataManager)
                : base(OverriddenConfigurationGroup, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<OverriddenConfigurationGroup> context)
            {
                OverriddenConfigurationGroup entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<OverriddenConfigurationGroup> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IOverriddenConfigurationGroupDataManager> context)
            {
                return context.DataManager.GetOverriddenConfigurationGroup();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<OverriddenConfigurationGroup, IOverriddenConfigurationGroupDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<OverriddenConfigurationGroup, IOverriddenConfigurationGroupDataManager> context)
            {
                return true;
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_OverriddenConfigurationGroup;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<OverriddenConfigurationGroup> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<OverriddenConfigurationGroup> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<OverriddenConfigurationGroup, IOverriddenConfigurationGroupDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<OverriddenConfigurationGroup> context)
            {
                context.Entity.OverriddenConfigurationGroupId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
