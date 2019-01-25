using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.RDBTests.Common;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
namespace Vanrise.RDB.Tests.Security
{
    [TestClass]
    public class TenantDataManagerTests
    {
        const string DBTABLE_NAME_Tenant = "Tenant";

        ITenantDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ITenantDataManager>();
        ITenantDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ITenantDataManager>();
        [TestMethod]
        public void AddUpdateSelectTenants()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Security, DBTABLE_NAME_Tenant);
            Tenant Tenant = new Tenant
            {
                Settings = new TenantSettings(),
                Name = "RDB TEST",
            };
            var tester = new TenantMainOperationTester(Tenant, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class TenantMainOperationTester : EntityMainOperationTester<Tenant, ITenantDataManager>
        {
            public TenantMainOperationTester(Tenant Tenant, ITenantDataManager rdbDataManager, ITenantDataManager sqlDataManager)
                : base(Tenant, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<Tenant> context)
            {
                Tenant entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<Tenant> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<ITenantDataManager> context)
            {
                return context.DataManager.GetTenants();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<Tenant, ITenantDataManager> context)
            {
                return context.DataManager.AddTenant(context.Entity, out int insertedid);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<Tenant, ITenantDataManager> context)
            {
                return context.DataManager.UpdateTenant(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Security;

            public override string DBTableName => DBTABLE_NAME_Tenant;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<Tenant> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<Tenant> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<Tenant, ITenantDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<Tenant> context)
            {
            }
        }

        #endregion
    }
}
