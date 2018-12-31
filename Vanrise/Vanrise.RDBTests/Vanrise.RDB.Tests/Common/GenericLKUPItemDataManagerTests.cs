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
    public class GenericLKUPItemDataManagerTests
    {
        const string DBTABLE_NAME_GenericLKUPItem = "GenericLKUP";

        IGenericLKUPItemDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IGenericLKUPItemDataManager>();
        IGenericLKUPItemDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IGenericLKUPItemDataManager>();

        [TestMethod]
        public void AddUpdateSelectGenericLKUPItems()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_GenericLKUPItem);
            GenericLKUPItem GenericLKUPItem = new GenericLKUPItem
            {
                GenericLKUPItemId = Guid.NewGuid(),
                Name = "RDB TEST",
                BusinessEntityDefinitionId = Guid.NewGuid(),
                Settings = new GenericLKUPItemSettings
                {
                },
            };
            var tester = new GenericLKUPItemMainOperationTester(GenericLKUPItem, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class GenericLKUPItemMainOperationTester : EntityMainOperationTester<GenericLKUPItem, IGenericLKUPItemDataManager>
        {
            public GenericLKUPItemMainOperationTester(GenericLKUPItem GenericLKUPItem, IGenericLKUPItemDataManager rdbDataManager, IGenericLKUPItemDataManager sqlDataManager)
                : base(GenericLKUPItem, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<GenericLKUPItem> context)
            {
                GenericLKUPItem entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.Settings = new GenericLKUPItemSettings();
            }
            public override IEnumerable<GenericLKUPItem> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IGenericLKUPItemDataManager> context)
            {
               return context.DataManager.GetGenericLKUPItem();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<GenericLKUPItem, IGenericLKUPItemDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<GenericLKUPItem, IGenericLKUPItemDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_GenericLKUPItem;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<GenericLKUPItem> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<GenericLKUPItem> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<GenericLKUPItem, IGenericLKUPItemDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<GenericLKUPItem> context)
            {
                context.Entity.GenericLKUPItemId = Guid.NewGuid();
            }
        }
        #endregion 

    }
}
