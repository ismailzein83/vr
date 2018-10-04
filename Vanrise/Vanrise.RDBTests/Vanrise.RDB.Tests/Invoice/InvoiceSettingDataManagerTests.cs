using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.Invoice.Data;
using Vanrise.RDBTests.Common;
using Vanrise.Invoice.Entities;
using System.Collections.Generic;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class InvoiceSettingDataManagerTests
    {
        const string DBTABLE_NAME_INVOICESETTING = "InvoiceSetting";

        IInvoiceSettingDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();
        IInvoiceSettingDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();

        [TestMethod]
        public void AddUpdateSelectInvoiceSettings()
        {
            InvoiceSetting invoiceSetting = new InvoiceSetting();
            invoiceSetting.InvoiceSettingId = Guid.NewGuid();
            invoiceSetting.Name = string.Format(" UT Type {0}", invoiceSetting.InvoiceSettingId.ToString().Substring(0, 15));
            invoiceSetting.InvoiceTypeId = Guid.NewGuid();
            invoiceSetting.Details = new InvoiceSettingDetails { InvoiceSettingParts = new Dictionary<Guid, InvoiceSettingPart>() };
            var tester = new InvoiceSettingMainOperationTester(invoiceSetting, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, true);
        }

        #region Private Classes

        private class InvoiceSettingMainOperationTester : EntityMainOperationTester<InvoiceSetting, IInvoiceSettingDataManager>
        {
            public InvoiceSettingMainOperationTester(InvoiceSetting InvoiceSetting, IInvoiceSettingDataManager rdbDataManager, IInvoiceSettingDataManager sqlDataManager)
                : base(InvoiceSetting, rdbDataManager, sqlDataManager)
            {

            }
            
            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<InvoiceSetting> context)
            {
                InvoiceSetting entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.IsDefault = true;
                entity.Details.InvoiceSettingParts.Add(new Guid("B96140F0-9F30-4E44-BF80-41AC693996AF"), new MyInvoiceSettingPart { Prop = "fsdgfdsg gfds fdsg'fdg" });
            }

            private class MyInvoiceSettingPart : InvoiceSettingPart
            {
                public string Prop { get; set; }

                public override Guid ConfigId => new Guid("F202B6D4-53C5-458F-8314-F2D78D2D6821");
            }
            
            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<InvoiceSetting> context)
            {
                context.Entity.InvoiceSettingId = Guid.NewGuid();
            }

            public override IEnumerable<InvoiceSetting> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IInvoiceSettingDataManager> context)
            {
                return context.DataManager.GetInvoiceSettings();
            }
            
            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<InvoiceSetting, IInvoiceSettingDataManager> context)
            {
                return context.DataManager.InsertInvoiceSetting(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<InvoiceSetting, IInvoiceSettingDataManager> context)
            {
                return context.DataManager.UpdateInvoiceSetting(context.Entity);
            }
            
            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_INVOICE;

            public override string DBTableName => DBTABLE_NAME_INVOICESETTING;

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<InvoiceSetting, IInvoiceSettingDataManager> context)
            {
                context.DataManager.DeleteInvoiceSetting(context.Entity.InvoiceSettingId);
            }

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<InvoiceSetting> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<InvoiceSetting> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }
        }

        #endregion
    }
}
