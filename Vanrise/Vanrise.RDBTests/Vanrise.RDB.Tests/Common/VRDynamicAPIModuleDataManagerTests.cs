using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.Common
{
    [TestClass]
    public class VRDynamicAPIModuleModuleDataManagerTests
    {
        const string DBTABLE_NAME_VRDynamicAPIModule = "VRDynamicAPIModule";

        IVRDynamicAPIModuleDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IVRDynamicAPIModuleDataManager>();
        IVRDynamicAPIModuleDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IVRDynamicAPIModuleDataManager>();

        [TestMethod]
        public void AddUpdateSelectVRDynamicAPIModules()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_VRDynamicAPIModule);
            VRDynamicAPIModule VRDynamicAPIModule = new VRDynamicAPIModule
            {
                Name = "RDB TEST",
                CreatedBy = 1,
                LastModifiedBy = 1
            };
            var tester = new VRDynamicAPIModuleMainOperationTester(VRDynamicAPIModule, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class VRDynamicAPIModuleMainOperationTester : EntityMainOperationTester<VRDynamicAPIModule, IVRDynamicAPIModuleDataManager>
        {
            public VRDynamicAPIModuleMainOperationTester(VRDynamicAPIModule VRDynamicAPIModule, IVRDynamicAPIModuleDataManager rdbDataManager, IVRDynamicAPIModuleDataManager sqlDataManager)
                : base(VRDynamicAPIModule, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<VRDynamicAPIModule> context)
            {
                VRDynamicAPIModule entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<VRDynamicAPIModule> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IVRDynamicAPIModuleDataManager> context)
            {
                var apis = context.DataManager.GetVRDynamicAPIModules();
                if (apis == null)
                    return null;
                foreach (var api in apis)
                {
                    api.CreatedTime = api.CreatedTime.Date;
                    api.LastModifiedTime = api.LastModifiedTime.Date;
                }
                return apis;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<VRDynamicAPIModule, IVRDynamicAPIModuleDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<VRDynamicAPIModule, IVRDynamicAPIModuleDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_VRDynamicAPIModule;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<VRDynamicAPIModule> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<VRDynamicAPIModule> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<VRDynamicAPIModule, IVRDynamicAPIModuleDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<VRDynamicAPIModule> context)
            {
            }
        }

        #endregion
    }
}
