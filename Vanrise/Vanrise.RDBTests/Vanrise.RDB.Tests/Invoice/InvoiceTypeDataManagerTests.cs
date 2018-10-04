using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.Invoice.Data;
using Vanrise.Common;
using System.Collections.Generic;
using Vanrise.Invoice.Entities;
using System.Linq;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class InvoiceTypeDataManagerTests
    {
        const string DBTABLE_NAME_INVOICETYPE = "InvoiceType";

        IInvoiceTypeDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceTypeDataManager>();
        IInvoiceTypeDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceTypeDataManager>();
          
        [TestMethod]
        public void AddUpdateSelectInvoiceType()
        {
            InvoiceType invoiceType = new InvoiceType();
            invoiceType.InvoiceTypeId = Guid.NewGuid();
            invoiceType.Name = string.Format(" UT Type {0}", invoiceType.InvoiceTypeId);
            invoiceType.Settings = new InvoiceTypeSettings { CurrencyFieldName = "fff" };
            var tester = new InvoiceTypeMainOperationTester(invoiceType, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest();
        }

        #region Private Classes

        private class InvoiceTypeMainOperationTester : EntityMainOperationTester<InvoiceType, IInvoiceTypeDataManager>
        {
            public InvoiceTypeMainOperationTester(InvoiceType invoiceType, IInvoiceTypeDataManager rdbDataManager, IInvoiceTypeDataManager sqlDataManager)
                : base(invoiceType, rdbDataManager, sqlDataManager)
            {

            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_INVOICE;

            public override string DBTableName => DBTABLE_NAME_INVOICETYPE;
            
            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<InvoiceType> context)
            {
                InvoiceType invType = context.Entity;
                invType.Name = invType.Name + " Updated ";
                invType.Settings.StagesToHoldNames = new List<string> { "fdsfdsf" };
            }
            
            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<InvoiceType, IInvoiceTypeDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<InvoiceType> context)
            {
                context.Entity.InvoiceTypeId = Guid.NewGuid();
            }

            public override IEnumerable<InvoiceType> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IInvoiceTypeDataManager> context)
            {
                return context.DataManager.GetInvoiceTypes();
            }
            
            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<InvoiceType, IInvoiceTypeDataManager> context)
            {
                return context.DataManager.InsertInvoiceType(context.Entity);
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<InvoiceType> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<InvoiceType> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<InvoiceType, IInvoiceTypeDataManager> context)
            {
                return context.DataManager.UpdateInvoiceType(context.Entity);
            }
        }

        #endregion
    }
}
