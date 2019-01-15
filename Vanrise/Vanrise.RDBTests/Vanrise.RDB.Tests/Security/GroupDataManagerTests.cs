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
    public class GroupDataManagerTests
    {
        const string DBTABLE_NAME_Group = "Group";

        IGroupDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IGroupDataManager>();
        IGroupDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IGroupDataManager>();
        [TestMethod]
        public void AddUpdateSelectGroups()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Security, DBTABLE_NAME_Group);
            Group Group = new Group
            {
                Name = "RDB TEST",
                Description = Guid.NewGuid().ToString(),
                GroupId = 1,
                TenantId = 1
            };
            var tester = new GroupMainOperationTester(Group, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);

             _rdbDataManager.AssignMembers(1,new[]{1});
            _sqlDataManager.AssignMembers(1, new[] { 1 });

            var rdbUserGroups = _rdbDataManager.GetUserGroups(1);
            var sqlUserGroups = _sqlDataManager.GetUserGroups(1);
            UTUtilities.AssertObjectsAreSimilar(rdbUserGroups, sqlUserGroups);
        }

        #region Private Classes

        private class GroupMainOperationTester : EntityMainOperationTester<Group, IGroupDataManager>
        {
            public GroupMainOperationTester(Group Group, IGroupDataManager rdbDataManager, IGroupDataManager sqlDataManager)
                : base(Group, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<Group> context)
            {
                Group entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<Group> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IGroupDataManager> context)
            {
                return context.DataManager.GetGroups();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<Group, IGroupDataManager> context)
            {
                return context.DataManager.AddGroup(context.Entity, out int userid);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<Group, IGroupDataManager> context)
            {
                return context.DataManager.UpdateGroup(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Security;

            public override string DBTableName => DBTABLE_NAME_Group;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<Group> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<Group> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<Group, IGroupDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<Group> context)
            {
            }
        }

        #endregion
    }
}
