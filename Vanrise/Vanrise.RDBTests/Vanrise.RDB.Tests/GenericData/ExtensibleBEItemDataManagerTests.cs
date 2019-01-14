using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.RDBTests.Common;
using Vanrise.GenericData.Entities;
namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class ExtensibleBEItemDataManagerTests
    {
        const string DBTABLE_NAME_ExtensibleBEItem = "ExtensibleBEItem";

        IExtensibleBEItemDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IExtensibleBEItemDataManager>();
        IExtensibleBEItemDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IExtensibleBEItemDataManager>();
        //[TestMethod]
        //public void AddUpdateSelectExtensibleBEItems()
        //{
        //    UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_ExtensibleBEItem);
        //    ExtensibleBEItem ExtensibleBEItem = new ExtensibleBEItem
        //    {
        //        ExtensibleBEItemId = Guid.NewGuid(),
        //        Sections = new List<GenericEditorSection>(),
        //        DataRecordTypeId = Guid.NewGuid(),
        //    };
        //    var tester = new ExtensibleBEItemMainOperationTester(ExtensibleBEItem, _rdbDataManager, _sqlDataManager);
        //    tester.ExecuteTest(true, true, false);
        //}

        #region Private Classes

        private class ExtensibleBEItemMainOperationTester : EntityMainOperationTester<ExtensibleBEItem, IExtensibleBEItemDataManager>
        {
            public ExtensibleBEItemMainOperationTester(ExtensibleBEItem ExtensibleBEItem, IExtensibleBEItemDataManager rdbDataManager, IExtensibleBEItemDataManager sqlDataManager)
                : base(ExtensibleBEItem, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<ExtensibleBEItem> context)
            {
                ExtensibleBEItem entity = context.Entity;
                entity.Sections = new List<GenericEditorSection>();
            }
            public override IEnumerable<ExtensibleBEItem> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IExtensibleBEItemDataManager> context)
            {
                return context.DataManager.GetExtensibleBEItems();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<ExtensibleBEItem, IExtensibleBEItemDataManager> context)
            {
                return context.DataManager.AddExtensibleBEItem(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<ExtensibleBEItem, IExtensibleBEItemDataManager> context)
            {
                return context.DataManager.UpdateExtensibleBEItem(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_ExtensibleBEItem;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<ExtensibleBEItem> context)
            {
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<ExtensibleBEItem> context)
            {
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<ExtensibleBEItem, IExtensibleBEItemDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<ExtensibleBEItem> context)
            {
                context.Entity.ExtensibleBEItemId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
