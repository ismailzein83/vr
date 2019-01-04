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
    public class VRMailMessageTypeDataManagerTests
    {
        const string DBTABLE_NAME_VRMailMessageType = "MailMessageType";

        IVRMailMessageTypeDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();
        IVRMailMessageTypeDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();

        [TestMethod]
        public void AddUpdateSelectVRMailMessageTypes()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_VRMailMessageType);
            VRMailMessageType VRMailMessageType = new VRMailMessageType
            {
                Name = "RDB TEST",
                Settings = new VRMailMessageTypeSettings
                {
                },
                VRMailMessageTypeId = Guid.NewGuid()
            };
            var tester = new VRMailMessageTypeMainOperationTester(VRMailMessageType, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class VRMailMessageTypeMainOperationTester : EntityMainOperationTester<VRMailMessageType, IVRMailMessageTypeDataManager>
        {
            public VRMailMessageTypeMainOperationTester(VRMailMessageType VRMailMessageType, IVRMailMessageTypeDataManager rdbDataManager, IVRMailMessageTypeDataManager sqlDataManager)
                : base(VRMailMessageType, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<VRMailMessageType> context)
            {
                VRMailMessageType entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.Settings = new VRMailMessageTypeSettings();
            }
            public override IEnumerable<VRMailMessageType> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IVRMailMessageTypeDataManager> context)
            {
                return context.DataManager.GetMailMessageTypes();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<VRMailMessageType, IVRMailMessageTypeDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<VRMailMessageType, IVRMailMessageTypeDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_VRMailMessageType;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<VRMailMessageType> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<VRMailMessageType> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<VRMailMessageType, IVRMailMessageTypeDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<VRMailMessageType> context)
            {
                context.Entity.VRMailMessageTypeId = Guid.NewGuid();
            }
        }
        #endregion
    }
}
