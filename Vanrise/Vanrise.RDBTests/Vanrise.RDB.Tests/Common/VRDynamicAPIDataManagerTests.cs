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
    public class VRDynamicAPIDataManagerTests
    {
        const string DBTABLE_NAME_VRDynamicAPI = "VRDynamicAPI";

        IVRDynamicAPIDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
        IVRDynamicAPIDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();

        [TestMethod]
        public void AddUpdateSelectVRDynamicAPIs()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_VRDynamicAPI);
            VRDynamicAPI VRDynamicAPI = new VRDynamicAPI
            {
                Name = "RDB TEST",
                ModuleId = Guid.NewGuid(),
                Settings = new VRDynamicAPISettings
                {
                },
                VRDynamicAPIId = Guid.NewGuid(),
                CreatedBy = 1,
                LastModifiedBy = 1
            };
            var tester = new VRDynamicAPIMainOperationTester(VRDynamicAPI, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class VRDynamicAPIMainOperationTester : EntityMainOperationTester<VRDynamicAPI, IVRDynamicAPIDataManager>
        {
            public VRDynamicAPIMainOperationTester(VRDynamicAPI VRDynamicAPI, IVRDynamicAPIDataManager rdbDataManager, IVRDynamicAPIDataManager sqlDataManager)
                : base(VRDynamicAPI, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<VRDynamicAPI> context)
            {
                VRDynamicAPI entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.Settings = new VRDynamicAPISettings
                {
                };
            }
            public override IEnumerable<VRDynamicAPI> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IVRDynamicAPIDataManager> context)
            {
                var apis = context.DataManager.GetVRDynamicAPIs();
                if (apis == null)
                    return null;
                foreach (var api in apis)
                {
                    api.CreatedTime = api.CreatedTime.Date;
                    api.LastModifiedTime = api.LastModifiedTime.Date;
                }
                return apis;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<VRDynamicAPI, IVRDynamicAPIDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<VRDynamicAPI, IVRDynamicAPIDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_VRDynamicAPI;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<VRDynamicAPI> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<VRDynamicAPI> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<VRDynamicAPI, IVRDynamicAPIDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<VRDynamicAPI> context)
            {
            }
        }

        #endregion
    }
}
