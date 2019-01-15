//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.RDBTests.Common;
//using Vanrise.Security.Data;
//using Vanrise.Security.Entities;
//namespace Vanrise.RDB.Tests.Security
//{
//    [TestClass]
//    public class BusinessEntityDataManagerTests
//    {
//        const string DBTABLE_NAME_BusinessEntity = "BusinessEntity";

//        IBusinessEntityDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
//        IBusinessEntityDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
//        [TestMethod]
//        public void AddUpdateSelectBusinessEntities()
//        {
//            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Security, DBTABLE_NAME_BusinessEntity);
//            BusinessEntity BusinessEntity = new BusinessEntity
//            {
//                BreakInheritance = false,
//                Name = "RDB TEST",
//                EntityId = Guid.NewGuid(),
//                ModuleId = Guid.NewGuid(),
//                PermissionOptions = new List<string>(),
//                Title = "RDB Tests",
//            };
//            var tester = new BusinessEntityMainOperationTester(BusinessEntity, _rdbDataManager, _sqlDataManager);
//            tester.ExecuteTest(true, true, false);
//            _rdbDataManager.ToggleBreakInheritance(BusinessEntity.EntityId);


//        }

//        #region Private Classes

//        private class BusinessEntityMainOperationTester : EntityMainOperationTester<BusinessEntity, IBusinessEntityDataManager>
//        {
//            public BusinessEntityMainOperationTester(BusinessEntity BusinessEntity, IBusinessEntityDataManager rdbDataManager, IBusinessEntityDataManager sqlDataManager)
//                : base(BusinessEntity, rdbDataManager, sqlDataManager)
//            {

//            }

//            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<BusinessEntity> context)
//            {
//                BusinessEntity entity = context.Entity;
//                entity.Name = entity.Name + " Updated ";
//            }
//            public override IEnumerable<BusinessEntity> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IBusinessEntityDataManager> context)
//            {
//                return context.DataManager.GetEntities();
//            }

//            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<BusinessEntity, IBusinessEntityDataManager> context)
//            {
//                return context.DataManager.AddBusinessEntity(context.Entity);
//            }

//            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<BusinessEntity, IBusinessEntityDataManager> context)
//            {
//                return context.DataManager.UpdateBusinessEntity(context.Entity);
//            }

//            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

//            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Security;

//            public override string DBTableName => DBTABLE_NAME_BusinessEntity;

//            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<BusinessEntity> context)
//            {
//                context.Entity.Name = Guid.NewGuid().ToString();
//            }

//            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<BusinessEntity> context)
//            {
//                context.TargetEntity.Name = context.SourceEntity.Name;
//            }

//            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<BusinessEntity, IBusinessEntityDataManager> context)
//            {
//                throw new NotImplementedException();
//            }

//            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<BusinessEntity> context)
//            {
//                context.Entity.EntityId = Guid.NewGuid();
//            }
//        }

//        #endregion
//    }
//}
