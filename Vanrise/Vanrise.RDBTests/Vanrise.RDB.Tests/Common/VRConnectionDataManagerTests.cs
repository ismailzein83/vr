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
    public class VRConnectionDataManagerTests
    {
        const string DBTABLE_NAME_VRConnection = "Connection";
        IVRConnectionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IVRConnectionDataManager>();
        IVRConnectionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IVRConnectionDataManager>();

        [TestMethod]
        public void AddUpdateSelectVRConnections()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_VRConnection);
            VRConnection VRConnection = new VRConnection
            {
                Name = "RDB TEST",
                VRConnectionId = Guid.NewGuid(),
                Settings = new VRConnectionSettingsImp()
        };
            var tester = new VRConnectionMainOperationTester(VRConnection, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes
        private class VRConnectionSettingsImp : VRConnectionSettings
        {
            public override Guid ConfigId { get { return new Guid("2385C389-D0D7-4852-979D-13641531CB24"); } }
        }
        private class VRConnectionMainOperationTester : EntityMainOperationTester<VRConnection, IVRConnectionDataManager>
        {
            public VRConnectionMainOperationTester(VRConnection VRConnection, IVRConnectionDataManager rdbDataManager, IVRConnectionDataManager sqlDataManager)
                : base(VRConnection, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<VRConnection> context)
            {
                VRConnection entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.Settings = new VRConnectionSettingsImp();
            }
            
            public override IEnumerable<VRConnection> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IVRConnectionDataManager> context)
            {
                return context.DataManager.GetVRConnections();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<VRConnection, IVRConnectionDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<VRConnection, IVRConnectionDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_VRConnection;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<VRConnection> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<VRConnection> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<VRConnection, IVRConnectionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<VRConnection> context)
            {
                context.Entity.VRConnectionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
