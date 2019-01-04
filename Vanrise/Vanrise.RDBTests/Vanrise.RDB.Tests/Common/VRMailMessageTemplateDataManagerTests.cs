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
    public class VRMailMessageTemplateDataManagerTests
    {
        const string DBTABLE_NAME_VRMailMessageTemplate = "MailMessageTemplate";

        IVRMailMessageTemplateDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IVRMailMessageTemplateDataManager>();
        IVRMailMessageTemplateDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IVRMailMessageTemplateDataManager>();

        [TestMethod]
        public void AddUpdateSelectVRMailMessageTemplates()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_VRMailMessageTemplate);
            VRMailMessageTemplate VRMailMessageTemplate = new VRMailMessageTemplate
            {
                Name = "RDB TEST",
                Settings = new VRMailMessageTemplateSettings
                {
                },
                VRMailMessageTemplateId = Guid.NewGuid(),
                VRMailMessageTypeId = Guid.NewGuid()
            };
            var tester = new VRMailMessageTemplateMainOperationTester(VRMailMessageTemplate, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class VRMailMessageTemplateMainOperationTester : EntityMainOperationTester<VRMailMessageTemplate, IVRMailMessageTemplateDataManager>
        {
            public VRMailMessageTemplateMainOperationTester(VRMailMessageTemplate VRMailMessageTemplate, IVRMailMessageTemplateDataManager rdbDataManager, IVRMailMessageTemplateDataManager sqlDataManager)
                : base(VRMailMessageTemplate, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<VRMailMessageTemplate> context)
            {
                VRMailMessageTemplate entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.Settings = new VRMailMessageTemplateSettings();
            }
            public override IEnumerable<VRMailMessageTemplate> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IVRMailMessageTemplateDataManager> context)
            {
                var mailMessageTemplates = context.DataManager.GetMailMessageTemplates();
                if (mailMessageTemplates == null)
                    return null;
                foreach (var mailMessageTemplate in mailMessageTemplates)
                {
                    mailMessageTemplate.CreatedTime = mailMessageTemplate.CreatedTime.Date;
                }
                return mailMessageTemplates;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<VRMailMessageTemplate, IVRMailMessageTemplateDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<VRMailMessageTemplate, IVRMailMessageTemplateDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_VRMailMessageTemplate;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<VRMailMessageTemplate> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<VRMailMessageTemplate> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<VRMailMessageTemplate, IVRMailMessageTemplateDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<VRMailMessageTemplate> context)
            {
                context.Entity.VRMailMessageTemplateId = Guid.NewGuid();
            }
        }
        #endregion
    }
}
