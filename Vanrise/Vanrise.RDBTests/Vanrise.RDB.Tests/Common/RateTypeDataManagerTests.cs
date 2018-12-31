using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.RDBTests.Common;
using Vanrise.Entities;
namespace Vanrise.RDB.Tests.Common
{
    [TestClass]
    public class RateTypeDataManagerTests
    {
        const string DBTABLE_NAME_RateType = "RateType";

        IRateTypeDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IRateTypeDataManager>();
        IRateTypeDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IRateTypeDataManager>();

        [TestMethod]
        public void AddUpdateSelectRateTypes()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_RateType);
            RateType RateType = new RateType
            {
                Name = "RDB TEST",
            };
            var tester = new RateTypeMainOperationTester(RateType, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class RateTypeMainOperationTester : EntityMainOperationTester<RateType, IRateTypeDataManager>
        {
            public RateTypeMainOperationTester(RateType RateType, IRateTypeDataManager rdbDataManager, IRateTypeDataManager sqlDataManager)
                : base(RateType, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<RateType> context)
            {
                RateType entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<RateType> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IRateTypeDataManager> context)
            {
                return context.DataManager.GetRateTypes();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<RateType, IRateTypeDataManager> context)
            {
                return context.DataManager.Insert(context.Entity, out int id);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<RateType, IRateTypeDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_RateType;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<RateType> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<RateType> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<RateType, IRateTypeDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<RateType> context)
            {
            }
        }
        #endregion 
    }
}
