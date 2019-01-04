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
    public class VRObjectTypeDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_VRObjectTypeDefinition = "VRObjectTypeDefinition";

        IVRObjectTypeDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();
        IVRObjectTypeDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();

        [TestMethod]
        public void AddUpdateSelectVRObjectTypeDefinitions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_VRObjectTypeDefinition);
            VRObjectTypeDefinition VRObjectTypeDefinition = new VRObjectTypeDefinition
            {
                Name = "RDB TEST",
                VRObjectTypeDefinitionId = Guid.NewGuid(),
                Settings = new VRObjectTypeDefinitionSettings
                {
                },
            };
            var tester = new VRObjectTypeDefinitionMainOperationTester(VRObjectTypeDefinition, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class VRObjectTypeDefinitionMainOperationTester : EntityMainOperationTester<VRObjectTypeDefinition, IVRObjectTypeDefinitionDataManager>
        {
            public VRObjectTypeDefinitionMainOperationTester(VRObjectTypeDefinition VRObjectTypeDefinition, IVRObjectTypeDefinitionDataManager rdbDataManager, IVRObjectTypeDefinitionDataManager sqlDataManager)
                : base(VRObjectTypeDefinition, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<VRObjectTypeDefinition> context)
            {
                VRObjectTypeDefinition entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<VRObjectTypeDefinition> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IVRObjectTypeDefinitionDataManager> context)
            {
                return context.DataManager.GetVRObjectTypeDefinitions();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<VRObjectTypeDefinition, IVRObjectTypeDefinitionDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<VRObjectTypeDefinition, IVRObjectTypeDefinitionDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_VRObjectTypeDefinition;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<VRObjectTypeDefinition> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<VRObjectTypeDefinition> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<VRObjectTypeDefinition, IVRObjectTypeDefinitionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<VRObjectTypeDefinition> context)
            {
                context.Entity.VRObjectTypeDefinitionId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
