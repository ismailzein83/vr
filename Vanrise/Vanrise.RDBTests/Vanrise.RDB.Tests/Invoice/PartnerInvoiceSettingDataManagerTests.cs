using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.RDBTests.Common;
using System.Collections.Generic;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class PartnerInvoiceSettingDataManagerTests
    {
        const string DBTABLE_NAME_PARTNERINVOICESETTING = "PartnerInvoiceSetting";

        IPartnerInvoiceSettingDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IPartnerInvoiceSettingDataManager>();
        IPartnerInvoiceSettingDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IPartnerInvoiceSettingDataManager>();

        [TestMethod]
        public void AddUpdateSelectPartnerInvoiceSettings()
        {
            PartnerInvoiceSetting partnerInvoiceSetting = new PartnerInvoiceSetting();
            partnerInvoiceSetting.PartnerInvoiceSettingId = Guid.NewGuid();
            partnerInvoiceSetting.InvoiceSettingID = Guid.NewGuid();
            partnerInvoiceSetting.PartnerId = "fdskfjnds kgf";
            partnerInvoiceSetting.Details = new PartnerInvoiceSettingDetails { InvoiceSettingParts = new Dictionary<Guid, InvoiceSettingPart>() };
            var tester = new PartnerInvoiceSettingMainOperationTester(partnerInvoiceSetting, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, false, true);
        }

        #region Private Classes

        private class PartnerInvoiceSettingMainOperationTester : EntityMainOperationTester<PartnerInvoiceSetting, IPartnerInvoiceSettingDataManager>
        {
            public PartnerInvoiceSettingMainOperationTester(PartnerInvoiceSetting PartnerInvoiceSetting, IPartnerInvoiceSettingDataManager rdbDataManager, IPartnerInvoiceSettingDataManager sqlDataManager)
                : base(PartnerInvoiceSetting, rdbDataManager, sqlDataManager)
            {

            }
            
            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<PartnerInvoiceSetting> context)
            {
                PartnerInvoiceSetting entity = context.Entity;
                entity.Details.InvoiceSettingParts.Add(new Guid("1FAA5207-E6F4-4DDF-9C0D-D7B5DABB9EE8"), new MyInvoicePart { Prop = "fdsf sgd fdsgt" });
            }

            private class MyInvoicePart : InvoiceSettingPart
            {
                public override Guid ConfigId => new Guid("DDD5D1CF-495E-45FA-AE9F-BBD1B71DC827");

                public String Prop { get; set; }
            }
            
            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<PartnerInvoiceSetting> context)
            {
                context.Entity.PartnerInvoiceSettingId = Guid.NewGuid();
            }

            public override IEnumerable<PartnerInvoiceSetting> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IPartnerInvoiceSettingDataManager> context)
            {
                return context.DataManager.GetPartnerInvoiceSettings();
            }
            
            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<PartnerInvoiceSetting, IPartnerInvoiceSettingDataManager> context)
            {
                return context.DataManager.InsertPartnerInvoiceSetting(context.Entity.PartnerInvoiceSettingId, context.Entity.InvoiceSettingID, context.Entity.PartnerId, context.Entity.Details);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<PartnerInvoiceSetting, IPartnerInvoiceSettingDataManager> context)
            {
                return context.DataManager.UpdatePartnerInvoiceSetting(context.Entity.PartnerInvoiceSettingId, context.Entity.Details);
            }
            
            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_INVOICE;

            public override string DBTableName => DBTABLE_NAME_PARTNERINVOICESETTING;

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<PartnerInvoiceSetting, IPartnerInvoiceSettingDataManager> context)
            {
                context.DataManager.DeletePartnerInvoiceSetting(context.Entity.PartnerInvoiceSettingId);
            }

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<PartnerInvoiceSetting> context)
            {
                context.Entity.PartnerId = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<PartnerInvoiceSetting> context)
            {
                context.TargetEntity.PartnerId = context.SourceEntity.PartnerId;
            }
        }

        #endregion
    }
}
