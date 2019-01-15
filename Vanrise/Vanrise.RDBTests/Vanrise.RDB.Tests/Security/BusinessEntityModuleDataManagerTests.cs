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
    public class BusinessEntityModuleModuleDataManagerTests
    {
        const string DBTABLE_NAME_BusinessEntityModule = "BusinessEntityModule";

        IBusinessEntityModuleDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
        IBusinessEntityModuleDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
        [TestMethod]
        public void AddUpdateSelectBusinessEntityModules()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Security, DBTABLE_NAME_BusinessEntityModule);
            BusinessEntityModule businessEntityModule = new BusinessEntityModule
            {
                BreakInheritance = false,
                Name = "RDB TEST",
                ParentId = Guid.NewGuid(),
                ModuleId = Guid.NewGuid(),
            };
            var tester = new BusinessEntityModuleMainOperationTester(businessEntityModule, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
            var  rdbToggleBreakInheritance = _rdbDataManager.ToggleBreakInheritance(businessEntityModule.ModuleId);
            var sqlToggleBreakInheritance = _sqlDataManager.ToggleBreakInheritance(businessEntityModule.ModuleId);
            UTUtilities.AssertValuesAreEqual(rdbToggleBreakInheritance, sqlToggleBreakInheritance);

            var rdbBusinessEntityModuleRank = _rdbDataManager.UpdateBusinessEntityModuleRank(businessEntityModule.ModuleId, null);
            var sqlBusinessEntityModuleRank = _sqlDataManager.UpdateBusinessEntityModuleRank(businessEntityModule.ModuleId, null);
            UTUtilities.AssertValuesAreEqual(rdbBusinessEntityModuleRank, sqlBusinessEntityModuleRank);

        }

        #region Private Classes

        private class BusinessEntityModuleMainOperationTester : EntityMainOperationTester<BusinessEntityModule, IBusinessEntityModuleDataManager>
        {
            public BusinessEntityModuleMainOperationTester(BusinessEntityModule BusinessEntityModule, IBusinessEntityModuleDataManager rdbDataManager, IBusinessEntityModuleDataManager sqlDataManager)
                : base(BusinessEntityModule, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<BusinessEntityModule> context)
            {
                BusinessEntityModule entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<BusinessEntityModule> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IBusinessEntityModuleDataManager> context)
            {
                return context.DataManager.GetModules();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<BusinessEntityModule, IBusinessEntityModuleDataManager> context)
            {
                return context.DataManager.AddBusinessEntityModule(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<BusinessEntityModule, IBusinessEntityModuleDataManager> context)
            {
                return context.DataManager.UpdateBusinessEntityModule(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Security;

            public override string DBTableName => DBTABLE_NAME_BusinessEntityModule;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<BusinessEntityModule> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<BusinessEntityModule> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<BusinessEntityModule, IBusinessEntityModuleDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<BusinessEntityModule> context)
            {
                context.Entity.ModuleId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
