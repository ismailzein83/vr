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
        const string DBTABLE_NAME_INVOICEBULKACTIONDRAFT = "InvoiceBulkActionDraft";

        IInvoiceBulkActionsDraftDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceBulkActionsDraftDataManager>();
        IInvoiceBulkActionsDraftDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceBulkActionsDraftDataManager>();

        IInvoiceDataManager _rdbInvoiceDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceDataManager>();
        IInvoiceDataManager _sqlInvoiceDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceDataManager>();

        [TestMethod]
        public void InsertUpdateGetDeleteInvoiceBulkActionsDraft()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICEBULKACTIONDRAFT);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, InvoiceDataManagerTests.DBTABLE_NAME_INVOICE);
            Guid invoiceTypeId1 = Guid.NewGuid();
            TestInsertUpdateGet(invoiceTypeId1);
            Guid invoiceTypeId2 = Guid.NewGuid();
            TestInsertUpdateGet(invoiceTypeId2);
            Guid invoiceTypeId3 = Guid.NewGuid();
            TestInsertUpdateGet(invoiceTypeId3);
        }

        private void TestInsertUpdateGet(Guid invoiceTypeId)
        {
            List<int> invoiceIdsIndexes = new List<int> { 0, 1, 2 };
            UTUtilities.CallActionIteratively(
                (isAllSelected, clear1, clear2, firstInvoiceIds, secondInvoiceIds, thirdInvoiceIds) =>
                {
                    TestInsertUpdateGetDelete(invoiceTypeId, isAllSelected, clear1, clear2, firstInvoiceIds, secondInvoiceIds, thirdInvoiceIds);
                }, UTUtilities.GetBoolListForTesting(), UTUtilities.GetBoolListForTesting(), UTUtilities.GetBoolListForTesting(), invoiceIdsIndexes, invoiceIdsIndexes, invoiceIdsIndexes);
        }

        private void TestInsertUpdateGetDelete(Guid invoiceTypeId, bool isAllSelected, bool clear1, bool clear2, int firstInvoiceIds, int secondInvoiceIds, int thirdInvoiceIds)
        {
            Guid uniqueIdentifier = Guid.NewGuid();

            List<long> insertedInvoiceIds;
            InsertInvoices(invoiceTypeId, out insertedInvoiceIds);
            Random r = new Random();
            List<List<long>> invoiceIdsList = new List<List<long>>
            {
                null,
                new List<long>(),
                new List<long> { insertedInvoiceIds[r.Next(insertedInvoiceIds.Count)], insertedInvoiceIds[r.Next(insertedInvoiceIds.Count)], insertedInvoiceIds[r.Next(insertedInvoiceIds.Count)], insertedInvoiceIds[r.Next(insertedInvoiceIds.Count)], insertedInvoiceIds[r.Next(insertedInvoiceIds.Count)] }
            };

            TestUpdateDrafts(invoiceTypeId, isAllSelected, uniqueIdentifier, invoiceIdsList[firstInvoiceIds]);
            if (clear1)
                ClearDrafts(uniqueIdentifier);
            TestUpdateDrafts(invoiceTypeId, isAllSelected, uniqueIdentifier, invoiceIdsList[secondInvoiceIds]);
            if (clear2)
                ClearDrafts(uniqueIdentifier);
            TestUpdateDrafts(invoiceTypeId, isAllSelected, uniqueIdentifier, invoiceIdsList[thirdInvoiceIds]);
        }

        private void ClearDrafts(Guid uniqueIdentifier)
        {
            _sqlDataManager.ClearInvoiceBulkActionDrafts(uniqueIdentifier);
            _rdbDataManager.ClearInvoiceBulkActionDrafts(uniqueIdentifier);
        }

        private void TestUpdateDrafts(Guid invoiceTypeId, bool isAllSelected, Guid uniqueIdentifier, List<long> invoiceIds)
        {
            var sqlSummary = _sqlDataManager.UpdateInvoiceBulkActionDraft(uniqueIdentifier, invoiceTypeId, isAllSelected, invoiceIds);
            var rdbSummary = _rdbDataManager.UpdateInvoiceBulkActionDraft(uniqueIdentifier, invoiceTypeId, isAllSelected, invoiceIds);
            UTUtilities.AssertObjectsAreSimilar(sqlSummary, rdbSummary);
            AssertTablesAreSimilar();
            AssertDraftsAreSimilar(uniqueIdentifier);
        }

        private void AssertTablesAreSimilar()
        {
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICEBULKACTIONDRAFT);
        }

        private void InsertInvoices(Guid invoiceTypeId, out List<long> invoiceIds)
        {
            List<Vanrise.Invoice.Entities.GenerateInvoiceInputToSave> invoicesToSave = new List<Vanrise.Invoice.Entities.GenerateInvoiceInputToSave>();
            for (int i = 0; i < 50; i++)
            {
                invoicesToSave.Add(new Vanrise.Invoice.Entities.GenerateInvoiceInputToSave
                {
                    Invoice = new Vanrise.Invoice.Entities.Invoice
                    {
                        DueDate = DateTime.Now,
                        ToDate = DateTime.Now,
                        FromDate = DateTime.Now,
                        InvoiceTypeId = invoiceTypeId,
                        IssueDate = DateTime.Now,
                        PartnerId = Guid.NewGuid().ToString(),
                        SerialNumber = Guid.NewGuid().ToString(),
                        UserId = 3
                    },
                    SplitInvoiceGroupId = Guid.NewGuid(),
                    InvoiceIdToDelete = 3,
                    InvoiceItemSets = new List<Vanrise.Invoice.Entities.GeneratedInvoiceItemSet>()
                });
            }
            List<long> sqlInvoiceIds;
            List<long> rdbInvoiceIds;
            UTUtilities.AssertValuesAreEqual(_sqlInvoiceDataManager.SaveInvoices(invoicesToSave, out sqlInvoiceIds), _rdbInvoiceDataManager.SaveInvoices(invoicesToSave, out rdbInvoiceIds));
            UTUtilities.AssertObjectsAreSimilar(sqlInvoiceIds, rdbInvoiceIds);
            invoiceIds = rdbInvoiceIds;
        }

        private void AssertDraftsAreSimilar(Guid uniqueIdentifier)
        {
            List<Vanrise.Invoice.Entities.Invoice> sqlInvoices = new List<Vanrise.Invoice.Entities.Invoice>();
            List<Vanrise.Invoice.Entities.Invoice> rdbInvoices = new List<Vanrise.Invoice.Entities.Invoice>();

            _sqlDataManager.LoadInvoicesFromInvoiceBulkActionDraft(uniqueIdentifier, (inv) => sqlInvoices.Add(inv));
            _rdbDataManager.LoadInvoicesFromInvoiceBulkActionDraft(uniqueIdentifier, (inv) => rdbInvoices.Add(inv));

            sqlInvoices = sqlInvoices.OrderBy(itm => itm.InvoiceId).ToList();
            rdbInvoices = rdbInvoices.OrderBy(itm => itm.InvoiceId).ToList();

            InvoiceDataManagerTests.AssertInvoicesAreSimilar(sqlInvoices, rdbInvoices);
        }
    }
}
