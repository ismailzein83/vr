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
    public class VRTimeZoneDataManagerTests
    {
        const string DBTABLE_NAME_VRTimeZone = "VRTimeZone";

        IVRTimeZoneDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IVRTimeZoneDataManager>();
        IVRTimeZoneDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IVRTimeZoneDataManager>();

        [TestMethod]
        public void AddUpdateSelectVRTimeZones()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_VRTimeZone);
            Entities.VRTimeZone vrTimeZone = new Entities.VRTimeZone
            {
                Name = "RDB TEST",
                CreatedBy = 1,
                LastModifiedBy = 1,
                Settings = new VRTimeZoneSettings()
            };
            var tester = new VRTimeZoneMainOperationTester(vrTimeZone, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class VRTimeZoneMainOperationTester : EntityMainOperationTester<VRTimeZone, IVRTimeZoneDataManager>
        {
            public VRTimeZoneMainOperationTester(VRTimeZone VRTimeZone, IVRTimeZoneDataManager rdbDataManager, IVRTimeZoneDataManager sqlDataManager)
                : base(VRTimeZone, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<VRTimeZone> context)
            {
                VRTimeZone entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<VRTimeZone> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IVRTimeZoneDataManager> context)
            {
                var timezones = context.DataManager.GetVRTimeZones();
                if (timezones == null)
                    return null;
                foreach (var vrTimeZone in timezones)
                {
                    vrTimeZone.CreatedTime = vrTimeZone.CreatedTime.Date;
                    if (vrTimeZone.LastModifiedTime.HasValue)
                        vrTimeZone.LastModifiedTime = vrTimeZone.LastModifiedTime.Value.Date;
                }
                return timezones;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<VRTimeZone, IVRTimeZoneDataManager> context)
            {
                return context.DataManager.Insert(context.Entity, out int VRTimeZoneId);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<VRTimeZone, IVRTimeZoneDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_VRTimeZone;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<VRTimeZone> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<VRTimeZone> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<VRTimeZone, IVRTimeZoneDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<VRTimeZone> context)
            {
            }
        }

        #endregion
    }
}
