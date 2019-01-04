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
    public class BEParentChildRelationDataManagerTests
    {
        const string DBTABLE_NAME_BEParentChildRelation = "BEParentChildRelation";

        IBEParentChildRelationDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();
        IBEParentChildRelationDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();
        Guid parentChildDefinitionId = Guid.NewGuid();

        [TestMethod]
        public void AddUpdateSelectBEParentChildRelations()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_BEParentChildRelation);
            BEParentChildRelation BEParentChildRelation = new BEParentChildRelation
            {
                BED = DateTime.Now,
                ChildBEId = "1",
                EED = DateTime.Now,
                ParentBEId = "1",
                RelationDefinitionId = parentChildDefinitionId,
            };
            var tester = new BEParentChildRelationMainOperationTester(BEParentChildRelation, _rdbDataManager, _sqlDataManager, parentChildDefinitionId);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class BEParentChildRelationMainOperationTester : EntityMainOperationTester<BEParentChildRelation, IBEParentChildRelationDataManager>
        {
            Guid _parentChildDefinitionId;
            public BEParentChildRelationMainOperationTester(BEParentChildRelation BEParentChildRelation, IBEParentChildRelationDataManager rdbDataManager, IBEParentChildRelationDataManager sqlDataManager, Guid parentChildDefinitionId)
                : base(BEParentChildRelation, rdbDataManager, sqlDataManager)
            {
                _parentChildDefinitionId = parentChildDefinitionId;
            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<BEParentChildRelation> context)
            {
                BEParentChildRelation entity = context.Entity;
                entity.EED = DateTime.Now;
            }
            public override IEnumerable<BEParentChildRelation> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IBEParentChildRelationDataManager> context)
            {
                return context.DataManager.GetBEParentChildRelations(_parentChildDefinitionId);
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<BEParentChildRelation, IBEParentChildRelationDataManager> context)
            {
                return context.DataManager.Insert(context.Entity, out long insertedId);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<BEParentChildRelation, IBEParentChildRelationDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

            public override string DBTableName => DBTABLE_NAME_BEParentChildRelation;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<BEParentChildRelation> context)
            {
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<BEParentChildRelation> context)
            {
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<BEParentChildRelation, IBEParentChildRelationDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<BEParentChildRelation> context)
            {
            }
        }

        #endregion
    }
}
