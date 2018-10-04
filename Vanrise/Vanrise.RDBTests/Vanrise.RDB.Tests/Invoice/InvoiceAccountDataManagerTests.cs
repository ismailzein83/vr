using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.RDBTests.Common;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class InvoiceAccountDataManagerTests
    {
        internal const string DBTABLE_NAME_INVOICEACCOUNT = "InvoiceAccount";

        IInvoiceAccountDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
        IInvoiceAccountDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();

        [TestMethod]
        public void InsertUpdateGetInvoiceAccounts()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICEACCOUNT);

            TestAccount(new VRInvoiceAccount
            {
                InvoiceTypeId = Guid.NewGuid(),
                PartnerId = "Pdsaf "
            });

            Guid invoiceTypeId = Guid.NewGuid();
            var account1 = new VRInvoiceAccount
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = "partner1",
            };
            TestAccount(account1);
            TestAccount(account1.VRDeepCopy());
            var account2 = new VRInvoiceAccount
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = "partner2",
                BED = DateTime.Today.AddMonths(-2),
                EED = DateTime.Today.AddDays(10)
            };
            TestAccount(account2);
            TestAccount(account2.VRDeepCopy());
            var account3 = new VRInvoiceAccount
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = "partner3",
               Status = VRAccountStatus.InActive
            };
            TestAccount(account3);
            TestAccount(account3.VRDeepCopy());

            var account4 = new VRInvoiceAccount
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = "partner4",
                Status = VRAccountStatus.InActive,
                BED = DateTime.Today.AddMonths(-2),
                EED = DateTime.Today.AddDays(10)
            };
            TestAccount(account4);
            TestAccount(account4.VRDeepCopy());

            TestGetInvoiceAccounts(invoiceTypeId, null);
            TestGetInvoiceAccounts(invoiceTypeId, new List<string>());
            TestGetInvoiceAccounts(invoiceTypeId, new List<string> { account1.PartnerId, account2.PartnerId, account3.PartnerId, account4.PartnerId, account4.PartnerId });
        }

        private void TestAccount(VRInvoiceAccount account)
        {
            TestInsertAccount(account);

            TestUpdateAccountStatus(account.InvoiceTypeId, account.PartnerId, Entities.VRAccountStatus.Active, false);
            TestUpdateAccountStatus(account.InvoiceTypeId, account.PartnerId, Entities.VRAccountStatus.InActive, false);
            TestUpdateAccountStatus(account.InvoiceTypeId, account.PartnerId, Entities.VRAccountStatus.Active, true);
            TestUpdateAccountStatus(account.InvoiceTypeId, account.PartnerId, Entities.VRAccountStatus.InActive, true);
            TestUpdateAccountStatus(account.InvoiceTypeId, account.PartnerId, Entities.VRAccountStatus.Active, false);

            TestUpdateAccountEffectiveDate(account.InvoiceTypeId, account.PartnerId, null, null);
            TestUpdateAccountEffectiveDate(account.InvoiceTypeId, account.PartnerId, DateTime.Now.AddMonths(-5), null);
            TestUpdateAccountEffectiveDate(account.InvoiceTypeId, account.PartnerId, null, DateTime.Now.AddMonths(155));
            TestUpdateAccountEffectiveDate(account.InvoiceTypeId, account.PartnerId, DateTime.Now.AddMonths(-5), DateTime.Now.AddMonths(155));
            TestUpdateAccountEffectiveDate(account.InvoiceTypeId, account.PartnerId, null, null);
        }

        private void TestInsertAccount(VRInvoiceAccount account)
        {
            long sqlAccountId;
            long rdbAccountId;
            UTAssert.ObjectsAreEqual(_sqlDataManager.InsertInvoiceAccount(account, out sqlAccountId), _rdbDataManager.InsertInvoiceAccount(account, out rdbAccountId));
            UTAssert.ObjectsAreEqual(sqlAccountId, rdbAccountId);
            account.InvoiceAccountId = rdbAccountId;

            AssertAllAccountsAreSimilar();
        }

        private void TestUpdateAccountStatus(Guid invoiceTypeId, string accountId, VRAccountStatus status, bool isDeleted)
        {
            UTAssert.ObjectsAreEqual(_sqlDataManager.TryUpdateInvoiceAccountStatus(invoiceTypeId, accountId, status, isDeleted),
            _rdbDataManager.TryUpdateInvoiceAccountStatus(invoiceTypeId, accountId, status, isDeleted));

            AssertAllAccountsAreSimilar();
        }

        private void TestUpdateAccountEffectiveDate(Guid invoiceTypeId, string accountId, DateTime? bed, DateTime? eed)
        {
            UTAssert.ObjectsAreEqual(_sqlDataManager.TryUpdateInvoiceAccountEffectiveDate(invoiceTypeId, accountId, bed, eed),
            _rdbDataManager.TryUpdateInvoiceAccountEffectiveDate(invoiceTypeId, accountId, bed, eed));

            AssertAllAccountsAreSimilar();
        }

        private void TestGetInvoiceAccounts(Guid invoiceTypeId, IEnumerable<string> partnerIds)
        {
            var sqlAccounts = _sqlDataManager.GetInvoiceAccountsByPartnerIds(invoiceTypeId, partnerIds);
            var rdbAccounts = _rdbDataManager.GetInvoiceAccountsByPartnerIds(invoiceTypeId, partnerIds);

            UTAssert.ObjectsAreSimilar(sqlAccounts, rdbAccounts);
        }

        private void AssertAllAccountsAreSimilar()
        {
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICEACCOUNT);
        }        
    }
}
