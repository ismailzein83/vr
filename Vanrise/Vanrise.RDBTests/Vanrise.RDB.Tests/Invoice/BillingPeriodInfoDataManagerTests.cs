using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class BillingPeriodInfoDataManagerTests
    {
        const string DBTABLE_NAME_BILLINGPERIOD = "BillingPeriodInfo";
        IBillingPeriodInfoDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBillingPeriodInfoDataManager>();
        IBillingPeriodInfoDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBillingPeriodInfoDataManager>();

        [TestMethod]
        public void InsertUpdateGetBillingPeriod()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_BILLINGPERIOD);
            for (int i = 0; i < 50; i++)
            {
                TestOneRecord();
            }
        }

        private void TestOneRecord()
        {
            var billingPeriod = new BillingPeriodInfo
            {
                InvoiceTypeId = Guid.NewGuid(),
                PartnerId = "partner " + Guid.NewGuid().ToString(),
                NextPeriodStart = DateTime.Now
            };
            InsertOrUpdateBillingPeriodAndAssertSimilar(billingPeriod);
            
            billingPeriod.NextPeriodStart = DateTime.Today.AddDays(-5);
            InsertOrUpdateBillingPeriodAndAssertSimilar(billingPeriod);
            
            billingPeriod.NextPeriodStart = DateTime.Today.AddDays(-15);
            InsertOrUpdateBillingPeriodAndAssertSimilar(billingPeriod);
        }

        private void InsertOrUpdateBillingPeriodAndAssertSimilar(BillingPeriodInfo billingPeriod)
        {
            _sqlDataManager.InsertOrUpdateBillingPeriodInfo(billingPeriod);
            UTUtilities.AssertValuesAreEqual(true, _rdbDataManager.InsertOrUpdateBillingPeriodInfo(billingPeriod));
            AssertBillingPeriodTablesAreSimilar();
        }

        private void AssertBillingPeriodTablesAreSimilar()
        {
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_BILLINGPERIOD);
        }
    }
}
