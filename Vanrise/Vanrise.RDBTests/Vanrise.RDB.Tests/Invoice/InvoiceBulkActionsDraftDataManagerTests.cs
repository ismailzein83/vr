using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Data;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class InvoiceBulkActionsDraftDataManagerTests
    {
        IInvoiceBulkActionsDraftDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceBulkActionsDraftDataManager>();
        IInvoiceBulkActionsDraftDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceBulkActionsDraftDataManager>();

        [TestMethod]
        public void InsertUpdateGetDeleteInvoiceBulkActionsDraft()
        {
            TestInsertUpdateGetDelete(true);
            TestInsertUpdateGetDelete(false);
        }

        private void TestInsertUpdateGetDelete(bool isAllSelected)
        {
            Guid rdbUniqueIdentifier = Guid.NewGuid();
            Guid sqlUniqueIdentifier = Guid.NewGuid();

            Guid invoiceTypeId = new Guid("94645C5E-CB98-4CE5-A0D0-F58AA726E3BE");
            List<long> targetInvoicesIds = new List<long> { 8, 4359843759 };

            var sqlSummary = _sqlDataManager.UpdateInvoiceBulkActionDraft(sqlUniqueIdentifier, invoiceTypeId, isAllSelected, targetInvoicesIds);
            var rdbSummary = _rdbDataManager.UpdateInvoiceBulkActionDraft(rdbUniqueIdentifier, invoiceTypeId, isAllSelected, targetInvoicesIds);

            UTAssert.ObjectsAreSimilar(sqlSummary, rdbSummary);
            AssertDraftsAreSimilar(rdbUniqueIdentifier, sqlUniqueIdentifier);

            _sqlDataManager.ClearInvoiceBulkActionDrafts(sqlUniqueIdentifier);
            _sqlDataManager.ClearInvoiceBulkActionDrafts(rdbUniqueIdentifier);
            AssertDraftsAreSimilar(rdbUniqueIdentifier, sqlUniqueIdentifier);
        }

        private void AssertDraftsAreSimilar(Guid rdbUniqueIdentifier, Guid sqlUniqueIdentifier)
        {
            List<Vanrise.Invoice.Entities.Invoice> sqlInvoices = new List<Vanrise.Invoice.Entities.Invoice>();
            List<Vanrise.Invoice.Entities.Invoice> rdbInvoices = new List<Vanrise.Invoice.Entities.Invoice>();

            _sqlDataManager.LoadInvoicesFromInvoiceBulkActionDraft(sqlUniqueIdentifier, (inv) => sqlInvoices.Add(inv));
            _rdbDataManager.LoadInvoicesFromInvoiceBulkActionDraft(rdbUniqueIdentifier, (inv) => rdbInvoices.Add(inv));

            UTAssert.ObjectsAreSimilar(sqlInvoices, rdbInvoices);
        }
    }
}
