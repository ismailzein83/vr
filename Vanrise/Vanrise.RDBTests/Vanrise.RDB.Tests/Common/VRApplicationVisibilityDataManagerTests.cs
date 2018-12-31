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
    public class VRApplicationVisibilityDataManagerTests
    {
        const string DBTABLE_NAME_VRApplicationVisibility = "VRAppVisibility";

        IVRApplicationVisibilityDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IVRApplicationVisibilityDataManager>();
        IVRApplicationVisibilityDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IVRApplicationVisibilityDataManager>();

        [TestMethod]
        public void AddUpdateSelectVRApplicationVisibilities()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_VRApplicationVisibility);
            VRApplicationVisibility VRApplicationVisibility = new VRApplicationVisibility
            {
                Name = "RDB TEST",
                IsCurrent = true,
                Settings = new VRApplicationVisibilitySettings
                {
                  ModulesVisibility = new Dictionary<Guid, VRModuleVisibility>()
                },
               VRApplicationVisibilityId = Guid.NewGuid()
            };
            var tester = new VRApplicationVisibilityMainOperationTester(VRApplicationVisibility, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class VRApplicationVisibilityMainOperationTester : EntityMainOperationTester<VRApplicationVisibility, IVRApplicationVisibilityDataManager>
        {
            public VRApplicationVisibilityMainOperationTester(VRApplicationVisibility VRApplicationVisibility, IVRApplicationVisibilityDataManager rdbDataManager, IVRApplicationVisibilityDataManager sqlDataManager)
                : base(VRApplicationVisibility, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<VRApplicationVisibility> context)
            {
                VRApplicationVisibility entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.IsCurrent = true;
                entity.Settings = new VRApplicationVisibilitySettings
                {
                    ModulesVisibility = new Dictionary<Guid, VRModuleVisibility>()
                };
                entity.VRApplicationVisibilityId = Guid.NewGuid();
            }
            public override IEnumerable<VRApplicationVisibility> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IVRApplicationVisibilityDataManager> context)
            {
                var vrApplicationVisibilities = context.DataManager.GetVRApplicationVisibilities();
                if (vrApplicationVisibilities == null)
                    return null;
                return vrApplicationVisibilities;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<VRApplicationVisibility, IVRApplicationVisibilityDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<VRApplicationVisibility, IVRApplicationVisibilityDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_VRApplicationVisibility;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<VRApplicationVisibility> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<VRApplicationVisibility> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<VRApplicationVisibility, IVRApplicationVisibilityDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<VRApplicationVisibility> context)
            {
                context.Entity.VRApplicationVisibilityId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
