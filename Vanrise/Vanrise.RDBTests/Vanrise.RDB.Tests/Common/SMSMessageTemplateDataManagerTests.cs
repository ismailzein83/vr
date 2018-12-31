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
    public class SMSMessageTemplateDataManagerTests
    {
        const string DBTABLE_NAME_SMSMessageTemplate = "SMSMessageTemplate";

        ISMSMessageTemplateDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ISMSMessageTemplateDataManager>();
        ISMSMessageTemplateDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ISMSMessageTemplateDataManager>();

        [TestMethod]
        public void AddUpdateSelectSMSMessageTemplates()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_SMSMessageTemplate);
            SMSMessageTemplate SMSMessageTemplate = new SMSMessageTemplate
            {
                Name = "RDB TEST",
                SMSMessageTypeId = Guid.NewGuid(),
                Settings = new SMSMessageTemplateSettings
                {
                },
                SMSMessageTemplateId = Guid.NewGuid(),
                CreatedBy = 1,
                LastModifiedBy = 1
            };
            var tester = new SMSMessageTemplateMainOperationTester(SMSMessageTemplate, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class SMSMessageTemplateMainOperationTester : EntityMainOperationTester<SMSMessageTemplate, ISMSMessageTemplateDataManager>
        {
            public SMSMessageTemplateMainOperationTester(SMSMessageTemplate SMSMessageTemplate, ISMSMessageTemplateDataManager rdbDataManager, ISMSMessageTemplateDataManager sqlDataManager)
                : base(SMSMessageTemplate, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<SMSMessageTemplate> context)
            {
                SMSMessageTemplate entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.Settings = new SMSMessageTemplateSettings
                {
                };
                entity.SMSMessageTemplateId = Guid.NewGuid();
            }
            public override IEnumerable<SMSMessageTemplate> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<ISMSMessageTemplateDataManager> context)
            {
                var vrApplicationVisibilities = context.DataManager.GetSMSMessageTemplates();
                if (vrApplicationVisibilities == null)
                    return null;
                foreach (var vrApplicationVisibility in vrApplicationVisibilities)
                {
                    vrApplicationVisibility.CreatedTime = vrApplicationVisibility.CreatedTime.Date;
                    vrApplicationVisibility.LastModifiedTime = vrApplicationVisibility.LastModifiedTime.Date;
                }
                return vrApplicationVisibilities;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<SMSMessageTemplate, ISMSMessageTemplateDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<SMSMessageTemplate, ISMSMessageTemplateDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_SMSMessageTemplate;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<SMSMessageTemplate> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<SMSMessageTemplate> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<SMSMessageTemplate, ISMSMessageTemplateDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<SMSMessageTemplate> context)
            {
                context.Entity.SMSMessageTemplateId = Guid.NewGuid();
            }
        }

        #endregion
    }
}
