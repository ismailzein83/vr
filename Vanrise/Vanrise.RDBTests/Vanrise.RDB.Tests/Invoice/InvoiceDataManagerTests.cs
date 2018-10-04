using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.Invoice.Data;
using Vanrise.Common;
using Vanrise.Invoice.Entities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Entities;
using Vanrise.RDBTests.Common;
using Vanrise.Entities;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class InvoiceDataManagerTests
    {
        const string DBTABLE_NAME_INVOICE = "Invoice";
        const string DBTABLE_NAME_INVOICEITEM = "InvoiceItem";

        IInvoiceDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceDataManager>();
        IInvoiceDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceDataManager>();

        IInvoiceAccountDataManager _rdbInvoiceAccountDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
        IInvoiceAccountDataManager _sqlInvoiceAccountDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();

        IInvoiceItemDataManager _rdbInvoiceItemDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceItemDataManager>();
        IInvoiceItemDataManager _sqlInvoiceItemDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceItemDataManager>();

        #region SaveInvoices Classes

        private class InvoiceDetail
        {
            public string Prop { get; set; }
        }

        private class InvoiceItemDetail
        {
            public string Prop { get; set; }
        }

        [TestMethod]
        public void SaveInvoicesAndGet()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICE);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICEITEM);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, InvoiceAccountDataManagerTests.DBTABLE_NAME_INVOICEACCOUNT);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, AccountBalance.BillingTransactionTypeDataManagerTests.DBTABLE_NAME_BILLINGTRANSACTION);

            Guid invoiceTypeId1 = Guid.NewGuid();
            TestSaveInvoicesAndGet(invoiceTypeId1);

            Guid invoiceTypeId2 = Guid.NewGuid();
            TestSaveInvoicesAndGet(invoiceTypeId2);

            Guid invoiceTypeId3 = Guid.NewGuid();
            TestSaveInvoicesAndGet(invoiceTypeId3);

            Guid invoiceTypeId4 = Guid.NewGuid();
            TestSaveInvoicesAndGet(invoiceTypeId4);
        }

        private void TestSaveInvoicesAndGet(Guid invoiceTypeId)
        {
            List<GenerateInvoiceInputToSave> invoicesToSave = GenerateInvoicesToSave(invoiceTypeId);

            List<long> sqlInvoiceIds;
            List<long> rdbInvoiceIds;
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetInvoices(sqlInvoiceIds, rdbInvoiceIds);

            IEnumerable<string> itemSetNames = invoicesToSave.SelectMany(itm => itm.InvoiceItemSets.Select(itm2 => itm2.SetName)).Distinct();
            if (itemSetNames.Count() > 0)
            {
                var rdbInvoiceItems = _rdbInvoiceItemDataManager.GetInvoiceItemsByItemSetNames(rdbInvoiceIds, itemSetNames, CompareOperator.Equal);
                var sqlInvoiceItems = _sqlInvoiceItemDataManager.GetInvoiceItemsByItemSetNames(sqlInvoiceIds, itemSetNames, CompareOperator.Equal);
                AssertInvoiceItemsAreSimilar(sqlInvoiceItems, rdbInvoiceItems);
            }
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);

            TestGetFiltered(invoiceTypeId, invoicesToSave);
            TestCheckInvoiceOverlaping(invoiceTypeId, invoicesToSave);
            TestSetInvoicePaid(invoiceTypeId, invoicesToSave);
            TestSetInvoicePaidById(invoiceTypeId, invoicesToSave);
            //TestSetInvoicePaidBySourceId(invoiceTypeId, invoicesToSave);
            TestSetInvoiceSent(invoiceTypeId, invoicesToSave);
            TestSetInvoiceLocked(invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceNotes(invoiceTypeId, invoicesToSave);
            TestUpdate(invoiceTypeId, invoicesToSave);
        }

        private List<GenerateInvoiceInputToSave> GenerateInvoicesToSave(Guid invoiceTypeId)
        {
            List<GenerateInvoiceInputToSave> invoicesToSave = new List<GenerateInvoiceInputToSave>();

            var invoiceToSave1 = new GenerateInvoiceInputToSave
            {
                Invoice = new Vanrise.Invoice.Entities.Invoice
                {
                    ApprovedBy = 5,
                    ApprovedTime = DateTime.Now,
                    Details = new InvoiceDetail { Prop = "rewsrewr" },
                    DueDate = DateTime.Now,
                    FromDate = DateTime.Today.AddDays(-2),
                    ToDate = DateTime.Today,
                    InvoiceSettingId = Guid.NewGuid(),
                    InvoiceTypeId = invoiceTypeId,
                    IssueDate = DateTime.Today.AddDays(1),
                    LockDate = DateTime.Now,
                    PaidDate = DateTime.Now,
                    PartnerId = "gfsdhg",
                    SentDate = DateTime.Now,
                    SerialNumber = "gfsdgfhgdhhahdeaar",
                    Settings = new InvoiceSettings { FileId = 5 },
                    SourceId = "5435435",
                    UserId = 5,
                    IsAutomatic = true,
                    NeedApproval = true,
                    SettlementInvoiceId = 4,
                    Note = "tewt eryt er",
                    SplitInvoiceGroupId =Guid.NewGuid()
                },
                InvoiceItemSets = new List<GeneratedInvoiceItemSet>
                 {
                     new GeneratedInvoiceItemSet
                     {
                          SetName = "Set 1",
                          Items = new List<GeneratedInvoiceItem>
                          {
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 1 Item 1",
                                     Details = new InvoiceItemDetail { Prop = "5435fsf456"}
                               },
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 1 Item 2",
                                     Details = new InvoiceItemDetail { Prop = "ghtrytrwterwt"}
                               }
                          }
                     },
                     new GeneratedInvoiceItemSet
                     {
                          SetName = "Set 2",
                          Items = new List<GeneratedInvoiceItem>
                          {
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 2 Item 1",
                                     Details = new InvoiceItemDetail { Prop = "5435fsf456"}
                               },
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 2 Item 2",
                                     Details = new InvoiceItemDetail { Prop = "ghtrytrwterwt"}
                               }
                          }
                     }
                 },
                MappedTransactions = new List<Vanrise.AccountBalance.Entities.BillingTransaction>
                {
                    new Vanrise.AccountBalance.Entities.BillingTransaction
                    {
                         AccountId = "account 1",
                         AccountTypeId = Guid.NewGuid(),
                         Amount = 34.66M,
                         CurrencyId=4,
                         Notes="3254325",
                         Reference = "43254325",
                         Settings = new Vanrise.AccountBalance.Entities.BillingTransactionSettings { },
                         SourceId = "54325435",
                         TransactionTime = DateTime.Now,
                         AccountBillingTransactionId = 4,
                         TransactionTypeId = Guid.NewGuid()
                    },
                    new Vanrise.AccountBalance.Entities.BillingTransaction
                    {
                         AccountId = "account 2",
                         AccountTypeId = Guid.NewGuid(),
                         Amount = 64.6453M,
                         CurrencyId=4,
                         Notes="ter hyyter",
                         Reference = "werytrey ",
                         Settings = new Vanrise.AccountBalance.Entities.BillingTransactionSettings { },
                         SourceId = "eqrterqy rutuyt",
                         TransactionTime = DateTime.Now,
                         AccountBillingTransactionId = 2,
                         TransactionTypeId = Guid.NewGuid()
                    }
                },
                SplitInvoiceGroupId = Guid.NewGuid(),
                InvoiceIdToDelete = 6
            };
            invoicesToSave.Add(invoiceToSave1);

            var invoiceToSave2 = new GenerateInvoiceInputToSave
            {
                Invoice = new Vanrise.Invoice.Entities.Invoice
                {
                    Details = new InvoiceDetail { Prop = "rewsrewr" },
                    FromDate = DateTime.Today.AddDays(-20),
                    ToDate = DateTime.Today,
                    InvoiceSettingId = Guid.NewGuid(),
                    InvoiceTypeId = invoiceTypeId,
                    IssueDate = DateTime.Today.AddDays(1),
                    PartnerId = "gfsdhg",
                    SerialNumber = "gfsdgfhgdhhahdeaar",
                    UserId = 5,
                    DueDate =DateTime.Now
                },
                InvoiceItemSets = new List<GeneratedInvoiceItemSet>
                 {
                     new GeneratedInvoiceItemSet
                     {
                          SetName = "Set 1",
                          Items = new List<GeneratedInvoiceItem>
                          {
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 1 Item 1",
                                     Details = new InvoiceItemDetail { Prop = "5435fsf456"}
                               },
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 1 Item 2",
                                     Details = new InvoiceItemDetail { Prop = "ghtrytrwterwt"}
                               }
                          }
                     },
                     new GeneratedInvoiceItemSet
                     {
                          SetName = "Set 2",
                          Items = new List<GeneratedInvoiceItem>
                          {
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 2 Item 1",
                                     Details = new InvoiceItemDetail { Prop = "5435fsf456"}
                               },
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 2 Item 2",
                                     Details = new InvoiceItemDetail { Prop = "ghtrytrwterwt"}
                               }
                          }
                     }
                 },
                MappedTransactions = new List<Vanrise.AccountBalance.Entities.BillingTransaction>
                {
                    new Vanrise.AccountBalance.Entities.BillingTransaction
                    {
                         AccountId = "account 1",
                         AccountTypeId = Guid.NewGuid(),
                         Amount = 34.66M,
                         CurrencyId=4,
                         Notes="3254325",
                         Reference = "43254325",
                         Settings = new Vanrise.AccountBalance.Entities.BillingTransactionSettings { },
                         SourceId = "54325435",
                         TransactionTime = DateTime.Now,
                         AccountBillingTransactionId = 4,
                         TransactionTypeId = Guid.NewGuid()
                    },
                    new Vanrise.AccountBalance.Entities.BillingTransaction
                    {
                         AccountId = "account 2",
                         AccountTypeId = Guid.NewGuid(),
                         Amount = 64.6453M,
                         CurrencyId=4,
                         Notes="ter hyyter",
                         Reference = "werytrey ",
                         Settings = new Vanrise.AccountBalance.Entities.BillingTransactionSettings { },
                         SourceId = "eqrterqy rutuyt",
                         TransactionTime = DateTime.Now,
                         AccountBillingTransactionId = 2,
                         TransactionTypeId = Guid.NewGuid()
                    }
                },
                SplitInvoiceGroupId = Guid.NewGuid(),
                InvoiceIdToDelete = 3,
                InvoiceToSettleIds = new List<long> { 5, 23 }
            };
            invoicesToSave.Add(invoiceToSave2);

            var invoiceToSave3 = new GenerateInvoiceInputToSave
            {
                Invoice = new Vanrise.Invoice.Entities.Invoice
                {
                    ApprovedBy = 5,
                    ApprovedTime = DateTime.Now,
                    Details = new InvoiceDetail { Prop = "rewsrewr" },
                    DueDate = DateTime.Now,
                    FromDate = DateTime.Now.AddDays(-25),
                    ToDate = DateTime.Now.AddDays(-5),
                    InvoiceSettingId = Guid.NewGuid(),
                    InvoiceTypeId = invoiceTypeId,
                    IsAutomatic = true,
                    IssueDate = DateTime.Now.AddDays(1),
                    LockDate = DateTime.Now,
                    NeedApproval = true,
                    Note = "dstgfdsgfdsg",
                    PaidDate = DateTime.Now,
                    PartnerId = "Account 3",
                    SentDate = DateTime.Now,
                    SerialNumber = "gfsdgfhgdhhahdeaar",
                    Settings = new InvoiceSettings { FileId = 5 },
                    SettlementInvoiceId = 4,
                    SourceId = "5435435",
                    UserId = 5
                },
                InvoiceItemSets = new List<GeneratedInvoiceItemSet>
                 {
                     new GeneratedInvoiceItemSet
                     {
                          SetName = "Set 1",
                          Items = new List<GeneratedInvoiceItem>
                          {
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 1 Item 1",
                                     Details = new InvoiceItemDetail { Prop = "5435fsf456"}
                               },
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 1 Item 2",
                                     Details = new InvoiceItemDetail { Prop = "ghtrytrwterwt"}
                               }
                          }
                     },
                     new GeneratedInvoiceItemSet
                     {
                          SetName = "Set 2",
                          Items = new List<GeneratedInvoiceItem>
                          {
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 2 Item 1",
                                     Details = new InvoiceItemDetail { Prop = "5435fsf456"}
                               },
                               new GeneratedInvoiceItem
                               {
                                    Name = "Set 2 Item 2",
                                     Details = new InvoiceItemDetail { Prop = "ghtrytrwterwt"}
                               }
                          }
                     }
                 },
                MappedTransactions = new List<Vanrise.AccountBalance.Entities.BillingTransaction>
                {
                    new Vanrise.AccountBalance.Entities.BillingTransaction
                    {
                         AccountId = "account 1",
                         AccountTypeId = Guid.NewGuid(),
                         Amount = 34.66M,
                         CurrencyId=4,
                         Notes="3254325",
                         Reference = "43254325",
                         Settings = new Vanrise.AccountBalance.Entities.BillingTransactionSettings { },
                         SourceId = "54325435",
                         TransactionTime = DateTime.Now,
                         AccountBillingTransactionId = 4,
                         TransactionTypeId = Guid.NewGuid()
                    },
                    new Vanrise.AccountBalance.Entities.BillingTransaction
                    {
                         AccountId = "account 2",
                         AccountTypeId = Guid.NewGuid(),
                         Amount = 64.6453M,
                         CurrencyId=4,
                         Notes="ter hyyter",
                         Reference = "werytrey ",
                         Settings = new Vanrise.AccountBalance.Entities.BillingTransactionSettings { },
                         SourceId = "eqrterqy rutuyt",
                         TransactionTime = DateTime.Now,
                         AccountBillingTransactionId = 2,
                         TransactionTypeId = Guid.NewGuid()
                    }
                },
                SplitInvoiceGroupId = Guid.NewGuid(),
                InvoiceIdToDelete = 66,
                InvoiceToSettleIds = new List<long> { 3, 13, 34 }
            };
            invoicesToSave.Add(invoiceToSave3);

            foreach (var invoiceToSave in invoicesToSave)
            {
                long sqlAccountId;
                long rdbAccountId;
                UTAssert.ObjectsAreEqual(_sqlInvoiceAccountDataManager.InsertInvoiceAccount(new VRInvoiceAccount { InvoiceTypeId = invoiceToSave.Invoice.InvoiceTypeId, PartnerId = invoiceToSave.Invoice.PartnerId }, out sqlAccountId), _rdbInvoiceAccountDataManager.InsertInvoiceAccount(new VRInvoiceAccount { InvoiceTypeId = invoiceToSave.Invoice.InvoiceTypeId, PartnerId = invoiceToSave.Invoice.PartnerId }, out rdbAccountId));
                UTAssert.ObjectsAreEqual(sqlAccountId, rdbAccountId);
            }

            return invoicesToSave;
        }

        private void SaveInvoices(List<GenerateInvoiceInputToSave> invoicesToSave, out List<long> sqlInvoiceIds, out List<long> rdbInvoiceIds)
        {
            UTAssert.ObjectsAreSimilar(_sqlDataManager.SaveInvoices(invoicesToSave, out sqlInvoiceIds), _rdbDataManager.SaveInvoices(invoicesToSave, out rdbInvoiceIds));
            UTAssert.ObjectsAreSimilar(sqlInvoiceIds, rdbInvoiceIds);
            for(int i=0;i<invoicesToSave.Count;i++)
            {
                invoicesToSave[i].Invoice.InvoiceId = rdbInvoiceIds[i];
            }
            AssertAllTablesAreSimilar();
        }

        private void TestGetInvoices(List<long> sqlInvoiceIds, List<long> rdbInvoiceIds)
        {
            var rdbInvoices = _rdbDataManager.GetInvoices(rdbInvoiceIds);
            var sqlInvoices = _sqlDataManager.GetInvoices(sqlInvoiceIds);
            AssertInvoicesAreSimilar(sqlInvoices, rdbInvoices);
        }

        private void TestGetFiltered(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestGetFiltered(invoiceTypeId, invoicesToSave, false);
            TestGetFiltered(invoiceTypeId, invoicesToSave, true);
        }

        private void TestGetFiltered(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave, bool withSelectAll)
        {
            var query = new InvoiceQuery
            {
                InvoiceTypeId = invoiceTypeId,
                FromTime = DateTime.Today.AddDays(-15),
                IsSelectAll = withSelectAll
            };
            TestGetFiltered(query);
            query.PartnerIds = new List<string>();
            TestGetFiltered(query);
            query.PartnerIds = new List<string> { invoicesToSave[0].Invoice.PartnerId };
            TestGetFiltered(query);
            query.PartnerIds = new List<string> { invoicesToSave[0].Invoice.PartnerId, invoicesToSave[1].Invoice.PartnerId };
            TestGetFiltered(query);
            query.PartnerIds = new List<string> { invoicesToSave[0].Invoice.PartnerId, invoicesToSave[1].Invoice.PartnerId, invoicesToSave[2].Invoice.PartnerId };
            TestGetFiltered(query);
            query.Status = VRAccountStatus.Active;
            TestGetFiltered(query);
            query.EffectiveDate = DateTime.Now;
            TestGetFiltered(query);
            query.IsEffectiveInFuture = true;
            TestGetFiltered(query);
            query.IncludeAllFields = true;
            TestGetFiltered(query);
            query.IsPaid = true;
            TestGetFiltered(query);
            query.IsSent = true;
            TestGetFiltered(query);
            query.IssueDate = DateTime.Today;
            TestGetFiltered(query);
            query.ToTime = DateTime.Today;
            TestGetFiltered(query);
        }

        private void TestGetFiltered(InvoiceQuery query)
        {
            var input = new DataRetrievalInput<InvoiceQuery> { Query = query };
            AssertInvoicesAreSimilar(_sqlDataManager.GetFilteredInvoices(input), _rdbDataManager.GetFilteredInvoices(input));
        }

        void TestCheckInvoiceOverlaping(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestCheckInvoiceOverlaping(invoiceTypeId, null, DateTime.Now.AddDays(-4), DateTime.Now, null);
            TestCheckInvoiceOverlaping(invoiceTypeId, "dsgfdsgfdrhgshhresat", DateTime.Now.AddDays(-4), DateTime.Now, null);
            var invoice = invoicesToSave[1].Invoice;
            TestCheckInvoiceOverlaping(invoiceTypeId, invoice.PartnerId, invoice.FromDate.AddDays(-20), invoice.FromDate.AddDays(-5), invoice.InvoiceId);
            TestCheckInvoiceOverlaping(invoiceTypeId, invoice.PartnerId, invoice.FromDate.AddDays(-5), invoice.FromDate, invoice.InvoiceId);
            TestCheckInvoiceOverlaping(invoiceTypeId, invoice.PartnerId, invoice.FromDate, invoice.FromDate.AddDays(1), invoice.InvoiceId);
            TestCheckInvoiceOverlaping(invoiceTypeId, invoice.PartnerId, invoice.ToDate.AddDays(-3), invoice.ToDate.AddDays(-2), invoice.InvoiceId);
            TestCheckInvoiceOverlaping(invoiceTypeId, invoice.PartnerId, invoice.ToDate.AddDays(-2), invoice.ToDate, invoice.InvoiceId);
            TestCheckInvoiceOverlaping(invoiceTypeId, invoice.PartnerId, invoice.ToDate.AddDays(-2), invoice.ToDate.AddDays(5), invoice.InvoiceId);
            TestCheckInvoiceOverlaping(invoiceTypeId, invoice.PartnerId, invoice.ToDate.AddDays(2), invoice.ToDate.AddDays(5), invoice.InvoiceId);
            TestCheckInvoiceOverlaping(invoiceTypeId, invoice.PartnerId, invoice.FromDate.AddDays(-2), invoice.ToDate.AddDays(5), invoice.InvoiceId);
        }

        void TestCheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId)
        {
            var rdbResponse = _rdbDataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId, fromDate, toDate, invoiceId);
            var sqlResponse = _sqlDataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId, fromDate, toDate, invoiceId);

            UTAssert.ObjectsAreEqual(sqlResponse, rdbResponse);

            if(invoiceId.HasValue)
            {
                var rdbResponse2 = _rdbDataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId, fromDate, toDate, null);
                var sqlResponse2 = _sqlDataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId, fromDate, toDate, null);

                UTAssert.ObjectsAreEqual(sqlResponse2, rdbResponse2);
            }

            var rdbResponse3 = _rdbDataManager.CheckInvoiceOverlaping(invoiceTypeId, null, fromDate, toDate, invoiceId);
            var sqlResponse3 = _sqlDataManager.CheckInvoiceOverlaping(invoiceTypeId, null, fromDate, toDate, invoiceId);

            UTAssert.ObjectsAreEqual(sqlResponse3, rdbResponse3);
        }

        void TestSetInvoicePaid(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[1].Invoice;
            TestSetInvoicePaid(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestSetInvoicePaid(invoice.InvoiceId, DateTime.Now, invoiceTypeId, invoicesToSave);
            TestSetInvoicePaid(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestSetInvoicePaid(invoice.InvoiceId, DateTime.Now, invoiceTypeId, invoicesToSave);
        }

        void TestSetInvoicePaid(long invoiceId, DateTime? paidDate, Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            _rdbDataManager.SetInvoicePaid(invoiceId, paidDate);
            _sqlDataManager.SetInvoicePaid(invoiceId, paidDate);

            TestGetFiltered(invoiceTypeId, invoicesToSave);
            AssertAllInvoicesAreSimilar();
        }

        void TestSetInvoicePaidById(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[2].Invoice;
            TestSetInvoicePaidById(invoice.InvoiceId, DateTime.Now, invoiceTypeId, invoicesToSave);
        }

        void TestSetInvoicePaidById(long invoiceId, DateTime paidDate, Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            _rdbDataManager.UpdateInvoicePaidDateById(invoiceTypeId, invoiceId, paidDate);
            _sqlDataManager.UpdateInvoicePaidDateById(invoiceTypeId, invoiceId, paidDate);

            TestGetFiltered(invoiceTypeId, invoicesToSave);
            AssertAllInvoicesAreSimilar();
        }

        void TestSetInvoicePaidBySourceId(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave.First(itm => itm.Invoice.SourceId != null).Invoice;
            TestSetInvoicePaidBySourceId(invoice.SourceId, DateTime.Now, invoiceTypeId, invoicesToSave);
        }

        void TestSetInvoicePaidBySourceId(string invoiceSourceId, DateTime paidDate, Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            _rdbDataManager.UpdateInvoicePaidDateBySourceId(invoiceTypeId, invoiceSourceId, paidDate);
            _sqlDataManager.UpdateInvoicePaidDateBySourceId(invoiceTypeId, invoiceSourceId, paidDate);

            TestGetFiltered(invoiceTypeId, invoicesToSave);
            AssertAllInvoicesAreSimilar();
        }

        void TestSetInvoiceSent(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[1].Invoice;
           TestSetInvoiceSent(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
           TestSetInvoiceSent(invoice.InvoiceId, DateTime.Now, invoiceTypeId, invoicesToSave);
           TestSetInvoiceSent(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestSetInvoiceSent(invoice.InvoiceId, DateTime.Now, invoiceTypeId, invoicesToSave);
        }

        void TestSetInvoiceSent(long invoiceId, DateTime? sendDate, Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            _rdbDataManager.SetInvoiceSentDate(invoiceId, sendDate);
            _sqlDataManager.SetInvoiceSentDate(invoiceId, sendDate);

            TestGetFiltered(invoiceTypeId, invoicesToSave);
            AssertAllInvoicesAreSimilar();
        }

        void TestSetInvoiceLocked(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[1].Invoice;
            TestSetInvoiceLocked(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestSetInvoiceLocked(invoice.InvoiceId, DateTime.Now, invoiceTypeId, invoicesToSave);
            TestSetInvoiceLocked(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestSetInvoiceLocked(invoice.InvoiceId, DateTime.Now, invoiceTypeId, invoicesToSave);
        }

        void TestSetInvoiceLocked(long invoiceId, DateTime? lockedDate, Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            _rdbDataManager.SetInvoiceLocked(invoiceId, lockedDate);
            _sqlDataManager.SetInvoiceLocked(invoiceId, lockedDate);

            TestGetFiltered(invoiceTypeId, invoicesToSave);
            AssertAllInvoicesAreSimilar();
        }

        void TestUpdateInvoiceNotes(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[1].Invoice;
            TestUpdateInvoiceNotes(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceNotes(invoice.InvoiceId, " sfdgfg'fdh ", invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceNotes(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceNotes(invoice.InvoiceId, "sdgtfdsg \nsdgfdsg", invoiceTypeId, invoicesToSave);
        }

        void TestUpdateInvoiceNotes(long invoiceId, string notes, Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            _rdbDataManager.UpdateInvoiceNote(invoiceId, notes);
            _sqlDataManager.UpdateInvoiceNote(invoiceId, notes);

            TestGetFiltered(invoiceTypeId, invoicesToSave);
            AssertAllInvoicesAreSimilar();
        }

        public void TestUpdate(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {


            Guid newInvoiceTypeId = Guid.NewGuid();
            Guid newSettingId = Guid.NewGuid();
            Guid newSplitGroupid = Guid.NewGuid();
            DateTime now = DateTime.Now;
            var invoiceToUpdate = invoicesToSave[2].Invoice;
            invoiceToUpdate.ApprovedBy = 4;
            invoiceToUpdate.ApprovedTime = now;
            invoiceToUpdate.Details = new InvoiceDetail { Prop = "shgfedhteryert" };
            invoiceToUpdate.DueDate = now;
            invoiceToUpdate.FromDate = now;
            invoiceToUpdate.ToDate = now;
            invoiceToUpdate.InvoiceSettingId = newSettingId;
            invoiceToUpdate.InvoiceTypeId = newInvoiceTypeId;
            invoiceToUpdate.IsAutomatic = true;
            invoiceToUpdate.IssueDate = now;
            invoiceToUpdate.LockDate = now;
            invoiceToUpdate.NeedApproval = true;
            invoiceToUpdate.Note = "wersiytjhperwoiueirtheo";
            invoiceToUpdate.PaidDate = now;
            invoiceToUpdate.PartnerId = "gtewrtyterytert";
            invoiceToUpdate.SentDate = now;
            invoiceToUpdate.SerialNumber = "iewuthpiuerw iewr f98 uyfa";
            invoiceToUpdate.Settings = new InvoiceSettings { FileId = 439857 };
            invoiceToUpdate.SettlementInvoiceId = 5432;
            invoiceToUpdate.SourceId = "dsjhgfoijoierwj";
            invoiceToUpdate.SplitInvoiceGroupId = newSplitGroupid;
            invoiceToUpdate.UserId = 3249842;

            _rdbDataManager.Update(invoiceToUpdate);
            _sqlDataManager.Update(invoiceToUpdate);

            TestGetFiltered(invoiceTypeId, invoicesToSave);
            AssertAllInvoicesAreSimilar();
        }

        void AssertAllTablesAreSimilar()
        {
            AssertAllInvoicesAreSimilar();
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICEITEM);
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, InvoiceAccountDataManagerTests.DBTABLE_NAME_INVOICEACCOUNT);
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, AccountBalance.BillingTransactionTypeDataManagerTests.DBTABLE_NAME_BILLINGTRANSACTION);
        }

        private static void AssertAllInvoicesAreSimilar()
        {
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICE);
        }

        void AssertInvoicesAreSimilar(IEnumerable<Vanrise.Invoice.Entities.Invoice> sqlInvoices, IEnumerable<Vanrise.Invoice.Entities.Invoice> rdbInvoices)
        {
            foreach(var invoice in sqlInvoices)
            {
                invoice.CreatedTime = default(DateTime);
            }
            foreach(var invoice in rdbInvoices)
            {
                invoice.CreatedTime = default(DateTime);
            }
            UTAssert.ObjectsAreSimilar(sqlInvoices, rdbInvoices);
        }

        private static void AssertInvoiceItemsAreSimilar(IEnumerable<InvoiceItem> sqlInvoiceItems, IEnumerable<InvoiceItem> rdbInvoiceItems)
        {
            sqlInvoiceItems = sqlInvoiceItems.OrderBy(itm => itm.InvoiceItemId);
            rdbInvoiceItems = rdbInvoiceItems.OrderBy(itm => itm.InvoiceItemId);
            UTAssert.ObjectsAreSimilar(sqlInvoiceItems, rdbInvoiceItems);
        }

        #endregion
        
        [TestMethod]
        public void GetPartnerInvoicesByDate()
        {
            GetPartnerInvoicesByDate(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), new List<string> { "uiotyuuyytu", "gfsdhg" }, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetPartnerInvoicesByDate(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), new List<string> { "ygtjd", "ytruytu" }, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetPartnerInvoicesByDate(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), new List<string>(), DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetPartnerInvoicesByDate(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), new List<string>(), DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetPartnerInvoicesByDate(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), null, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetPartnerInvoicesByDate(Guid.NewGuid(), new List<string> { "uiotyuuyytu", "gfsdhg" }, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetPartnerInvoicesByDate(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), new List<string> { "ygtjd", "ytruytu" }, DateTime.Now, DateTime.Now);
            GetPartnerInvoicesByDate(Guid.NewGuid(), new List<string> { "fewew", "342" }, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetPartnerInvoicesByDate(Guid.NewGuid(), null, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetPartnerInvoicesByDate(Guid.Empty, null, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
        }

        void GetPartnerInvoicesByDate(Guid invoiceTypeId, List<string> partnerIds, DateTime fromDate, DateTime toDate)
        {
            var rdbResponse = _rdbDataManager.GetPartnerInvoicesByDate(invoiceTypeId, partnerIds, fromDate, toDate);
            var sqlResponse = _sqlDataManager.GetPartnerInvoicesByDate(invoiceTypeId, partnerIds, fromDate, toDate);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }

        [TestMethod]
        public void GetInvoiceCount()
        {
            GetInvoiceCount(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), "uiotyuuyytu", DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetInvoiceCount(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), "uiotyuuyytu", DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetInvoiceCount(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), "uiotyuuyytu", DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetInvoiceCount(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), null, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetInvoiceCount(Guid.NewGuid(), "uiotyuuyytu", DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetInvoiceCount(new Guid("95B3D604-5C81-4CD2-9EB2-05A4BCC59A02"), "uiotyuuyytu", DateTime.Now, DateTime.Now);
            GetInvoiceCount(Guid.NewGuid(), "uiotyuuyytu", DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetInvoiceCount(Guid.NewGuid(), null, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
            GetInvoiceCount(Guid.Empty, null, DateTime.Parse("2012-01-01"), DateTime.Parse("2020-01-01"));
        }

        void GetInvoiceCount(Guid invoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate)
        {
            var rdbResponse = _rdbDataManager.GetInvoiceCount(invoiceTypeId, partnerId, fromDate, toDate);
            var sqlResponse = _sqlDataManager.GetInvoiceCount(invoiceTypeId, partnerId, fromDate, toDate);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }

        [TestMethod]
        public void GetInvoices()
        {
            GetInvoices(false, 2, 65, 7);
            GetInvoices(false, 6, 435436, 8);
            GetInvoices(false, 2, null, null);
            GetInvoices(false, 2, 65, null);
            GetInvoices(false, null, null, null);
            GetInvoices(false, 2, null, 7);
            GetInvoices(false, 4354366543, 554363465, 23432465436);
            GetInvoices(true, 2, 65, 7);
        }

        void GetInvoices(bool nullInvoiceIds, long? invoiceId1, long? invoiceId2, long? invoiceId3)
        {
            List<long> invoiceIds;
            if (nullInvoiceIds)
            {
                invoiceIds = null;
            }
            else
            {
                invoiceIds = new List<long>();
                if (invoiceId1.HasValue)
                    invoiceIds.Add(invoiceId1.Value);
                if (invoiceId2.HasValue)
                    invoiceIds.Add(invoiceId2.Value);
                if (invoiceId3.HasValue)
                    invoiceIds.Add(invoiceId3.Value);
            }

            var rdbResponse = _rdbDataManager.GetInvoices(invoiceIds);
            var sqlResponse = _sqlDataManager.GetInvoices(invoiceIds);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }
               

        [TestMethod]
        public void LoadInvoicesAfterImportedId()
        {
            LoadInvoicesAfterImportedId("26EE240A-0E05-4D07-BE86-EB0653E23141", 0);
            LoadInvoicesAfterImportedId("26EE240A-0E05-4D07-BE86-EB0653E23141", 16);
            LoadInvoicesAfterImportedId("26EE240A-0E05-4D07-BE86-EB0653E23141", 54765476547);
            LoadInvoicesAfterImportedId("75710BDF-4846-4ABF-9B50-A75DB373FE70", 4);
        }

        void LoadInvoicesAfterImportedId(string invoiceTypeIdString, long lastImportedId)
        {
            Guid invoiceTypeId = new Guid(invoiceTypeIdString);
            List<Vanrise.Invoice.Entities.Invoice> rdbResponse = new List<Vanrise.Invoice.Entities.Invoice>();
            List<Vanrise.Invoice.Entities.Invoice> sqlResponse = new List<Vanrise.Invoice.Entities.Invoice>();

            _rdbDataManager.LoadInvoicesAfterImportedId(invoiceTypeId, lastImportedId, (inv) => rdbResponse.Add(inv));
            _sqlDataManager.LoadInvoicesAfterImportedId(invoiceTypeId, lastImportedId, (inv) => sqlResponse.Add(inv));

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }

        [TestMethod]
        public void GetUnPaidPartnerInvoices()
        {
            List<List<PartnerInvoiceType>> inputs = new List<List<PartnerInvoiceType>>
        {
            new List<PartnerInvoiceType>
            {
                new PartnerInvoiceType
                {
                     InvoiceTypeId = new Guid("26EE240A-0E05-4D07-BE86-EB0653E23141"),
                     PartnerId = "gfsdhg"
                },
                new PartnerInvoiceType
                {
                     InvoiceTypeId = new Guid("02F0FC12-CCF0-4D84-AD5A-4B41A3C18882"),
                     PartnerId = "gfsdhg"
                },
                new PartnerInvoiceType
                {
                     InvoiceTypeId = new Guid("DEDE69B7-3967-48EE-B627-9768FD0134FD"),
                     PartnerId = "gfsdhg"
                }
            },
            new List<PartnerInvoiceType>
            {
                new PartnerInvoiceType
                {
                     InvoiceTypeId = new Guid("26EE240A-0E05-4D07-BE86-EB0653E23141"),
                     PartnerId = "gfsdhg"
                }
            },
            new List<PartnerInvoiceType>
            {
            }
        };

            foreach (var input in inputs)
            {
                GetUnPaidPartnerInvoices(input);
            }
        }

        void GetUnPaidPartnerInvoices(List<PartnerInvoiceType> input)
        {
            var rdbResponse = _rdbDataManager.GetUnPaidPartnerInvoices(input);
            if (rdbResponse != null)
                rdbResponse = rdbResponse.OrderBy(inv => inv.InvoiceId);
            var sqlResponse = _sqlDataManager.GetUnPaidPartnerInvoices(input);
            if (sqlResponse != null)
                sqlResponse = sqlResponse.OrderBy(inv => inv.InvoiceId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }

        [TestMethod]
        public void GetLastInvoicesByPartners()
        {
            List<List<PartnerInvoiceType>> inputs = new List<List<PartnerInvoiceType>>
        {
            new List<PartnerInvoiceType>
            {
                new PartnerInvoiceType
                {
                     InvoiceTypeId = new Guid("26EE240A-0E05-4D07-BE86-EB0653E23141"),
                     PartnerId = "gfsdhg"
                },
                new PartnerInvoiceType
                {
                     InvoiceTypeId = new Guid("02F0FC12-CCF0-4D84-AD5A-4B41A3C18882"),
                     PartnerId = "gfsdhg"
                },
                new PartnerInvoiceType
                {
                     InvoiceTypeId = new Guid("DEDE69B7-3967-48EE-B627-9768FD0134FD"),
                     PartnerId = "gfsdhg"
                }
            },
            new List<PartnerInvoiceType>
            {
                new PartnerInvoiceType
                {
                     InvoiceTypeId = new Guid("26EE240A-0E05-4D07-BE86-EB0653E23141"),
                     PartnerId = "gfsdhg"
                }
            },
            new List<PartnerInvoiceType>
            {
            }
        };

            foreach (var input in inputs)
            {
                GetLastInvoicesByPartners(input);
            }
        }

        void GetLastInvoicesByPartners(List<PartnerInvoiceType> input)
        {
            var rdbResponse = _rdbDataManager.GetLastInvoicesByPartners(input);
            if (rdbResponse != null)
                rdbResponse = rdbResponse.OrderBy(itm => itm.InvoiceTypeId).ThenBy(itm => itm.PartnerId);
            var sqlResponse = _sqlDataManager.GetLastInvoicesByPartners(input);
            if (sqlResponse != null)
                sqlResponse = sqlResponse.OrderBy(itm => itm.InvoiceTypeId).ThenBy(itm => itm.PartnerId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }
        
        [TestMethod]
        public void GetInvoiceBySourceId()
        {
            GetInvoiceBySourceId("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "5435435");
            GetInvoiceBySourceId("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "fsgtg");
            GetInvoiceBySourceId("26EE240A-0E05-4D07-BE86-EB0653E23141", "gsfdg");
            GetInvoiceBySourceId("75710BDF-4846-4ABF-9B50-A75DB373FE70", null);
        }

        void GetInvoiceBySourceId(string invoiceTypeIdString, string sourceId)
        {
            Guid invoiceTypeId = new Guid(invoiceTypeIdString);

            var rdbResponse = _rdbDataManager.GetInvoiceBySourceId(invoiceTypeId, sourceId);
            var sqlResponse = _sqlDataManager.GetInvoiceBySourceId(invoiceTypeId, sourceId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);

        }

        [TestMethod]
        public void GetLastInvoice()
        {
            GetLastInvoice("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "gfsdhg");
            GetLastInvoice("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "fsgtg");
            GetLastInvoice("26EE240A-0E05-4D07-BE86-EB0653E23141", "gsfdg");
            GetLastInvoice("75710BDF-4846-4ABF-9B50-A75DB373FE70", null);
        }

        void GetLastInvoice(string invoiceTypeIdString, string partnerId)
        {
            Guid invoiceTypeId = new Guid(invoiceTypeIdString);

            var rdbResponse = _rdbDataManager.GetLastInvoice(invoiceTypeId, partnerId);
            var sqlResponse = _sqlDataManager.GetLastInvoice(invoiceTypeId, partnerId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);

        }

        [TestMethod]
        public void GetLasInvoices()
        {
            GetLasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "gfsdhg", DateTime.Now, 10);
            GetLasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "gfsdhg", DateTime.Parse("2012-01-01"), 10);
            GetLasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "gfsdhg", null, 10);
            GetLasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", null, null, 10);
            GetLasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", null, null, 0);
            GetLasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "gfsdhg", DateTime.Parse("2012-01-01"), 1);
            GetLasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "gfsdhg", DateTime.Parse("2012-01-01"), 0);
        }

        void GetLasInvoices(string invoiceTypeIdString, string partnerId, DateTime? beforeDate, int lastInvoices)
        {
            Guid invoiceTypeId = new Guid(invoiceTypeIdString);
            
            var rdbResponse = _rdbDataManager.GetLasInvoices(invoiceTypeId, partnerId, beforeDate, lastInvoices);
            var sqlResponse = _sqlDataManager.GetLasInvoices(invoiceTypeId, partnerId, beforeDate, lastInvoices);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }

        [TestMethod]
        public void GetInvoicesPopulatedPeriod()
        {
            GetInvoicesPopulatedPeriod("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "gfsdhg");
            GetInvoicesPopulatedPeriod("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "fsgtg");
            GetInvoicesPopulatedPeriod("26EE240A-0E05-4D07-BE86-EB0653E23141", "gsfdg");
            GetInvoicesPopulatedPeriod("75710BDF-4846-4ABF-9B50-A75DB373FE70", null);
        }

        void GetInvoicesPopulatedPeriod(string invoiceTypeIdString, string partnerId)
        {
            Guid invoiceTypeId = new Guid(invoiceTypeIdString);

            var rdbResponse = _rdbDataManager.GetInvoicesPopulatedPeriod(invoiceTypeId, partnerId);
            var sqlResponse = _sqlDataManager.GetInvoicesPopulatedPeriod(invoiceTypeId, partnerId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);

        }

        [TestMethod]
        public void CheckPartnerIfHasInvoices()
        {
            CheckPartnerIfHasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "gfsdhg");
            CheckPartnerIfHasInvoices("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", "fsgtg");
            CheckPartnerIfHasInvoices("26EE240A-0E05-4D07-BE86-EB0653E23141", "gsfdg");
            CheckPartnerIfHasInvoices("75710BDF-4846-4ABF-9B50-A75DB373FE70", null);
        }

        void CheckPartnerIfHasInvoices(string invoiceTypeIdString, string partnerId)
        {
            Guid invoiceTypeId = new Guid(invoiceTypeIdString);

            var rdbResponse = _rdbDataManager.CheckPartnerIfHasInvoices(invoiceTypeId, partnerId);
            var sqlResponse = _sqlDataManager.CheckPartnerIfHasInvoices(invoiceTypeId, partnerId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);

        }

        [TestMethod]
        public void GetInvoicesBySerialNumbers()
        {
            GetInvoicesBySerialNumbers("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", new List<string> { "spodgtupoerwituj poierut9drsaiugf 9fdsa gdf9g iu", "spodgtupoerwituj poierut9drsaiugf 9fdsa gdf9g iu" });
            GetInvoicesBySerialNumbers("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B", new List<string> { "spodgtupoerwituj poierut9drsaiugf 9fdsa gdf9g iu", "ghgehyterytryteryyter" });
            GetInvoicesBySerialNumbers("26EE240A-0E05-4D07-BE86-EB0653E23141", new List<string> { "gterygeryter", "hgeyteryery" });
            GetInvoicesBySerialNumbers("75710BDF-4846-4ABF-9B50-A75DB373FE70", new List<string>());
        }

        void GetInvoicesBySerialNumbers(string invoiceTypeIdString, List<string> serialNumbers)
        {
            Guid invoiceTypeId = new Guid(invoiceTypeIdString);
            
            var rdbResponse = _rdbDataManager.GetInvoicesBySerialNumbers(invoiceTypeId, serialNumbers);
            var sqlResponse = _sqlDataManager.GetInvoicesBySerialNumbers(invoiceTypeId, serialNumbers);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);

        }
        
        [TestMethod]
        public void UpdateInvoiceSettings()
        {
            Vanrise.Invoice.Entities.Invoice rdbInvoice;
            Vanrise.Invoice.Entities.Invoice sqlInvoice;
            CreateInvoicesToTestUpdate(out rdbInvoice, out sqlInvoice);

            var invoiceSettings = new Vanrise.Invoice.Entities.InvoiceSettings { FileId = 43534 };
            _rdbDataManager.UpdateInvoiceSettings(rdbInvoice.InvoiceId, invoiceSettings);
            _sqlDataManager.UpdateInvoiceSettings(sqlInvoice.InvoiceId, invoiceSettings);

            AssertInvoicesAreValidAfterInsertOrUpdate(rdbInvoice.InvoiceId, sqlInvoice.InvoiceId);
        }


        [TestMethod]
        public void SetInvoiceSentDate()
        {
            SetInvoiceSentDate(DateTime.Now);
            SetInvoiceSentDate(null);
        }

        void SetInvoiceSentDate(DateTime? sendDate)
        {
            Vanrise.Invoice.Entities.Invoice rdbInvoice;
            Vanrise.Invoice.Entities.Invoice sqlInvoice;
            CreateInvoicesToTestUpdate(out rdbInvoice, out sqlInvoice);
            
            _rdbDataManager.SetInvoiceSentDate(rdbInvoice.InvoiceId, sendDate);
            _sqlDataManager.SetInvoiceSentDate(sqlInvoice.InvoiceId, sendDate);

            AssertInvoicesAreValidAfterInsertOrUpdate(rdbInvoice.InvoiceId, sqlInvoice.InvoiceId);
        }

        [TestMethod]
        public void ApproveInvoice()
        {
            ApproveInvoice(DateTime.Now);
            ApproveInvoice(null);
        }

        void ApproveInvoice(DateTime? approvedDate)
        {
            Vanrise.Invoice.Entities.Invoice rdbInvoice;
            Vanrise.Invoice.Entities.Invoice sqlInvoice;
            CreateInvoicesToTestUpdate(out rdbInvoice, out sqlInvoice);

            int? approvedBy = null;
            if (approvedDate.HasValue)
            {
                approvedDate = DateTime.Now;
                approvedBy = 5;
            }

            _rdbDataManager.ApproveInvoice(rdbInvoice.InvoiceId, approvedDate, approvedBy);
            _sqlDataManager.ApproveInvoice(sqlInvoice.InvoiceId, approvedDate, approvedBy);

            AssertInvoicesAreValidAfterInsertOrUpdate(rdbInvoice.InvoiceId, sqlInvoice.InvoiceId);
        }
        
        [TestMethod]
        public void LoadInvoices()
        {
            LoadInvoices(new Guid("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B"), null, null, null, null);
            LoadInvoices(new Guid("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B"), DateTime.Parse("2012-01-01"), null, null, OrderDirection.Ascending);
            LoadInvoices(new Guid("88EDCA6C-AD0A-40F1-8813-E6B965C9EE0B"), DateTime.Parse("2012-01-01"), DateTime.Parse("2012-01-01"), null, null);
        }

        void LoadInvoices(Guid invoiceTypeId, DateTime? from, DateTime? to, RecordFilterGroup filterGroup, OrderDirection? orderDirection)
        {
            var rdbResponse = new List<Vanrise.Invoice.Entities.Invoice>();
            var sqlResponse = new List<Vanrise.Invoice.Entities.Invoice>();

            _rdbDataManager.LoadInvoices(invoiceTypeId, from, to, filterGroup, orderDirection, () => false,
                (inv) =>
                {
                    rdbResponse.Add(inv);
                });

            _sqlDataManager.LoadInvoices(invoiceTypeId, from, to, filterGroup, orderDirection, () => false,
                (inv) =>
                {
                    sqlResponse.Add(inv);
                });

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }

        #region Private Methods

        void CreateInvoicesToTestUpdate(out Vanrise.Invoice.Entities.Invoice rdbInvoice, out Vanrise.Invoice.Entities.Invoice sqlInvoice)
        {
            List<GenerateInvoiceInputToSave> invoicesToSave = new List<GenerateInvoiceInputToSave>();

            GenerateInvoiceInputToSave invoiceToSave = new GenerateInvoiceInputToSave
            {
                Invoice = new Vanrise.Invoice.Entities.Invoice
                {
                    Details = new InvoiceDetail { Prop = "rewsrewr" },
                    DueDate = DateTime.Now,
                    FromDate = DateTime.Today.AddDays(-2),
                    ToDate = DateTime.Today,
                    InvoiceSettingId = Guid.NewGuid(),
                    InvoiceTypeId = Guid.NewGuid(),
                    IsAutomatic = true,
                    IssueDate = DateTime.Today.AddDays(1),
                    NeedApproval = true,
                    PartnerId = "gfsdhg",
                    SerialNumber = "spodgtupoerwituj poierut9drsaiugf 9fdsa gdf9g iu",
                    Settings = new InvoiceSettings { FileId = 678 },
                    SourceId = "5435435",
                    UserId = 5
                },
                InvoiceItemSets = new List<GeneratedInvoiceItemSet>()
            };
            invoicesToSave.Add(invoiceToSave);
            List<long> invoiceIds;
            _rdbDataManager.SaveInvoices(invoicesToSave, out invoiceIds);
            var rdbInvoiceId = invoiceIds[0];
            _sqlDataManager.SaveInvoices(invoicesToSave, out invoiceIds);
            var sqlInvoiceId = invoiceIds[0];

            List<Vanrise.Invoice.Entities.Invoice> invoices = _rdbDataManager.GetInvoices(new List<long> { rdbInvoiceId, sqlInvoiceId });
            rdbInvoice = invoices[0];
            sqlInvoice = invoices[1];
        }

        void AssertInvoicesAreValidAfterInsertOrUpdate(long rdbInvoiceId, long sqlInvoiceId)
        {
            var invoices = _rdbDataManager.GetInvoices(new List<long> { rdbInvoiceId, sqlInvoiceId });
            AssertInvoicesAreValidAfterInsertOrUpdate(invoices[0], invoices[1]);
        }

        void AssertInvoicesAreValidAfterInsertOrUpdate(Vanrise.Invoice.Entities.Invoice rdbInvoice, Vanrise.Invoice.Entities.Invoice sqlInvoice)
        {
            rdbInvoice.InvoiceId = default(long);
            sqlInvoice.InvoiceId = default(long);
            rdbInvoice.CreatedTime = default(DateTime);
            sqlInvoice.CreatedTime = default(DateTime);
            rdbInvoice.SplitInvoiceGroupId = null;
            sqlInvoice.SplitInvoiceGroupId = null;
            UTAssert.ObjectsAreSimilar(sqlInvoice, rdbInvoice);
        }

        #endregion
    }
}