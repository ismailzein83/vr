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

        #endregion

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

        #region Private Methods

        private void TestSaveInvoicesAndGet(Guid invoiceTypeId)
        {
            List<GenerateInvoiceInputToSave> invoicesToSave = GenerateInvoicesToSave(invoiceTypeId);

            List<long> sqlInvoiceIds;
            List<long> rdbInvoiceIds;
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetMethods(invoiceTypeId, invoicesToSave);

            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetMethods(invoiceTypeId, invoicesToSave);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetMethods(invoiceTypeId, invoicesToSave);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetMethods(invoiceTypeId, invoicesToSave);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetMethods(invoiceTypeId, invoicesToSave);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetMethods(invoiceTypeId, invoicesToSave);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetMethods(invoiceTypeId, invoicesToSave);
            SaveInvoices(invoicesToSave, out sqlInvoiceIds, out rdbInvoiceIds);
            TestGetMethods(invoiceTypeId, invoicesToSave);

            TestSetInvoicePaid(invoiceTypeId, invoicesToSave);
            TestSetInvoicePaidById(invoiceTypeId, invoicesToSave);
            //TestSetInvoicePaidBySourceId(invoiceTypeId, invoicesToSave);
            TestSetInvoiceSent(invoiceTypeId, invoicesToSave);
            TestSetInvoiceLocked(invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceNotes(invoiceTypeId, invoicesToSave);
            TestUpdate(invoiceTypeId, invoicesToSave);
            TestApproveInvoice(invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceSettings(invoiceTypeId, invoicesToSave);
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

        #region Test Get

        void TestGetMethods(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            List<long> invoiceIds = invoicesToSave.Select(itm => itm.Invoice.InvoiceId).ToList();
            TestGetInvoices(invoiceIds);
            TestGetInvoices(null);
            TestGetInvoices(new List<long>());
            TestGetInvoiceItems(invoicesToSave, invoiceIds);
            //TestGetFiltered(invoiceTypeId, invoicesToSave);
            TestGetInvoicesCount(invoiceTypeId, invoicesToSave);
            TestGetInvoicesBySerialNumbers(invoiceTypeId, invoicesToSave);
            TestGetLastInvoicesByPartners(invoiceTypeId, invoicesToSave);
            TestGetLastInvoice(invoiceTypeId, invoicesToSave);
            TestGetInvoicesPopulatedPeriod(invoiceTypeId, invoicesToSave);
            TestCheckPartnerIfHasInvoices(invoiceTypeId, invoicesToSave);
            TestGetLasInvoices(invoiceTypeId, invoicesToSave);
            TestGetPartnerInvoicesByDate(invoiceTypeId, invoicesToSave);
            TestGetUnPaidPartnerInvoices(invoiceTypeId, invoicesToSave);
            TestLoadInvoicesAfterImportedId(invoiceTypeId, invoicesToSave);
            TestGetInvoiceBySourceId(invoiceTypeId, invoicesToSave);
            TestLoadInvoices(invoiceTypeId, invoicesToSave);

            TestCheckInvoiceOverlaping(invoiceTypeId, invoicesToSave);
        }

        private void TestGetInvoices(List<long> invoiceIds)
        {
            var rdbInvoices = _rdbDataManager.GetInvoices(invoiceIds);
            var sqlInvoices = _sqlDataManager.GetInvoices(invoiceIds);
            AssertInvoicesAreSimilar(sqlInvoices, rdbInvoices);
        }

        private void TestGetInvoiceItems(List<GenerateInvoiceInputToSave> invoicesToSave, List<long> invoiceIds)
        {
            IEnumerable<string> itemSetNames = invoicesToSave.SelectMany(itm => itm.InvoiceItemSets.Select(itm2 => itm2.SetName)).Distinct();
            if (itemSetNames.Count() > 0)
            {
                var rdbInvoiceItems = _rdbInvoiceItemDataManager.GetInvoiceItemsByItemSetNames(invoiceIds, itemSetNames, CompareOperator.Equal);
                var sqlInvoiceItems = _sqlInvoiceItemDataManager.GetInvoiceItemsByItemSetNames(invoiceIds, itemSetNames, CompareOperator.Equal);
                AssertInvoiceItemsAreSimilar(sqlInvoiceItems, rdbInvoiceItems);
            }
        }
       

        private void TestGetFiltered(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            List<List<string>> partnerIdList = new List<List<string>>
            {
                null,
                new List<string>(),
                new List<string>{"dfsdfgg", "y3rerygt43", invoicesToSave[0].Invoice.PartnerId },
                new List<string> { invoicesToSave[0].Invoice.PartnerId, invoicesToSave[1].Invoice.PartnerId, invoicesToSave[2].Invoice.PartnerId }
            };
            List<VRAccountStatus?> accountStatuses = new List<VRAccountStatus?> { null, VRAccountStatus.Active };
            List<DateTime> fromDates = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(-15) };
            List<DateTime?> toDates = new List<DateTime?> { null, DateTime.Now };
            List<DateTime?> effectiveDates = new List<DateTime?> { null, DateTime.Now };
            List<DateTime?> issueDates = new List<DateTime?> { null, invoicesToSave[0].Invoice.IssueDate};
            List<bool> boolList = UTUtilities.GetBoolListForTesting();
            List<bool?> nullableBoolList = UTUtilities.GetNullableBoolListForTesting();
            List<bool?> isEffectiveInFutureList = new List<bool?> { null, true };
            int counter = 0;
            UTUtilities.CallActionIteratively(
                (fromTime, toTime, partnerIds, accountStatus, effectiveDate, isEffectiveInFuture, nullableBool) =>
                {
                    counter++;
                    var query = new InvoiceQuery
                    {
                        InvoiceTypeId = invoiceTypeId,
                        FromTime = fromTime,
                        ToTime = toTime,
                        PartnerIds = partnerIds,
                        Status = accountStatus,
                        EffectiveDate = effectiveDate,
                        IsEffectiveInFuture = isEffectiveInFuture,
                        IsSelectAll = nullableBool,
                        IsPaid = nullableBool,
                        IsSent = nullableBool,
                        //IssueDate = issueDate
                    };
                    TestGetFiltered(query);
                },
                fromDates, toDates, partnerIdList, accountStatuses,
                effectiveDates, isEffectiveInFutureList, nullableBoolList);                
        }

        private void TestGetFiltered(InvoiceQuery query)
        {
            var input = new DataRetrievalInput<InvoiceQuery> { Query = query };
            AssertInvoicesAreSimilar(_sqlDataManager.GetFilteredInvoices(input), _rdbDataManager.GetFilteredInvoices(input));
        }


        void TestGetInvoicesCount(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[1].Invoice;
            List<Guid> invoiceTypeIds = new List<Guid> { invoiceTypeId };
            List<string> partnerIds = new List<string> { null, "", invoice.PartnerId };
            UTUtilities.CallActionIteratively(TestGetInvoicesCount, invoiceTypeIds, partnerIds, UTUtilities.GetNullableDateTimeListsForTesting(), UTUtilities.GetNullableDateTimeListsForTesting());
        }

        void TestGetInvoicesCount(Guid invoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate)
        {
            UTAssert.ObjectsAreEqual(_sqlDataManager.GetInvoiceCount(invoiceTypeId, partnerId, fromDate, toDate),
                _rdbDataManager.GetInvoiceCount(invoiceTypeId, partnerId, fromDate, toDate));
            UTAssert.ObjectsAreEqual(_sqlDataManager.GetInvoiceCount(invoiceTypeId, null, fromDate, toDate),
               _rdbDataManager.GetInvoiceCount(invoiceTypeId, null, fromDate, toDate));
        }

        void TestGetInvoicesBySerialNumbers(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestGetInvoicesBySerialNumbers(invoiceTypeId, new List<string>());
            TestGetInvoicesBySerialNumbers(invoiceTypeId, new List<string> { "dfsgfdfgsgf", "gfsgfdhghter", invoicesToSave[0].Invoice.SerialNumber });
            TestGetInvoicesBySerialNumbers(invoiceTypeId, invoicesToSave.Select(itm => itm.Invoice.SerialNumber));

        }

        void TestGetInvoicesBySerialNumbers(Guid invoiceTypeId, IEnumerable<string> serialNumbers)
        {
            AssertInvoicesAreSimilar(_sqlDataManager.GetInvoicesBySerialNumbers(invoiceTypeId, serialNumbers),
                _rdbDataManager.GetInvoicesBySerialNumbers(invoiceTypeId, serialNumbers));
        }

        void TestGetLastInvoicesByPartners(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestGetLastInvoicesByPartners(new List<PartnerInvoiceType>());
            TestGetLastInvoicesByPartners(new List<PartnerInvoiceType>
            {
                new PartnerInvoiceType
                {
                     InvoiceTypeId = invoiceTypeId,
                      PartnerId = ""
                },
                new PartnerInvoiceType
                {
                     InvoiceTypeId = invoiceTypeId,
                      PartnerId = "gdseryerytyterteruyterjh gfdsg"
                },
                new PartnerInvoiceType
                {
                     InvoiceTypeId = invoiceTypeId,
                      PartnerId = invoicesToSave[1].Invoice.PartnerId
                }
            });
            TestGetLastInvoicesByPartners(invoicesToSave.Select(itm => itm.Invoice.PartnerId).Distinct().Select(partnerId => new PartnerInvoiceType { InvoiceTypeId = invoiceTypeId, PartnerId = partnerId }).ToList());
        }

        void TestGetLastInvoicesByPartners(List<PartnerInvoiceType> input)
        {
            var rdbResponse = _rdbDataManager.GetLastInvoicesByPartners(input);
            if (rdbResponse != null)
                rdbResponse = rdbResponse.OrderBy(itm => itm.InvoiceTypeId).ThenBy(itm => itm.PartnerId);
            var sqlResponse = _sqlDataManager.GetLastInvoicesByPartners(input);
            if (sqlResponse != null)
                sqlResponse = sqlResponse.OrderBy(itm => itm.InvoiceTypeId).ThenBy(itm => itm.PartnerId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }

        void TestGetLastInvoice(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestGetLastInvoice(invoiceTypeId, (string)null);
            TestGetLastInvoice(invoiceTypeId, "");
            TestGetLastInvoice(invoiceTypeId, "gdsgfdsg");
            TestGetLastInvoice(invoiceTypeId, invoicesToSave[0].Invoice.PartnerId);
            TestGetLastInvoice(invoiceTypeId, invoicesToSave[1].Invoice.PartnerId);
            TestGetLastInvoice(invoiceTypeId, invoicesToSave[2].Invoice.PartnerId);
        }

        void TestGetLastInvoice(Guid invoiceTypeId, string partnerId)
        {
            var rdbResponse = _rdbDataManager.GetLastInvoice(invoiceTypeId, partnerId);
            var sqlResponse = _sqlDataManager.GetLastInvoice(invoiceTypeId, partnerId);

            AssertInvoiceIsSimilar(sqlResponse, rdbResponse);
        }


        void TestGetInvoicesPopulatedPeriod(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestGetInvoicesPopulatedPeriod(invoiceTypeId, (string)null);
            TestGetInvoicesPopulatedPeriod(invoiceTypeId, "");
            TestGetInvoicesPopulatedPeriod(invoiceTypeId, "gdsgfdsg");
            TestGetInvoicesPopulatedPeriod(invoiceTypeId, invoicesToSave[0].Invoice.PartnerId);
            TestGetInvoicesPopulatedPeriod(invoiceTypeId, invoicesToSave[1].Invoice.PartnerId);
            TestGetInvoicesPopulatedPeriod(invoiceTypeId, invoicesToSave[2].Invoice.PartnerId);
        }

        void TestGetInvoicesPopulatedPeriod(Guid invoiceTypeId, string partnerId)
        {
            var rdbResponse = _rdbDataManager.GetInvoicesPopulatedPeriod(invoiceTypeId, partnerId);
            var sqlResponse = _sqlDataManager.GetInvoicesPopulatedPeriod(invoiceTypeId, partnerId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);

        }

        void TestCheckPartnerIfHasInvoices(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestCheckPartnerIfHasInvoices(invoiceTypeId, (string)null);
            TestCheckPartnerIfHasInvoices(invoiceTypeId, "");
            TestCheckPartnerIfHasInvoices(invoiceTypeId, "gdsgfdsg");
            TestCheckPartnerIfHasInvoices(invoiceTypeId, invoicesToSave[0].Invoice.PartnerId);
            TestCheckPartnerIfHasInvoices(invoiceTypeId, invoicesToSave[1].Invoice.PartnerId);
            TestCheckPartnerIfHasInvoices(invoiceTypeId, invoicesToSave[2].Invoice.PartnerId);
        }

        void TestCheckPartnerIfHasInvoices(Guid invoiceTypeId, string partnerId)
        {
            var rdbResponse = _rdbDataManager.CheckPartnerIfHasInvoices(invoiceTypeId, partnerId);
            var sqlResponse = _sqlDataManager.CheckPartnerIfHasInvoices(invoiceTypeId, partnerId);

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }

        void TestGetLasInvoices(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            List<Guid> invoiceTypeIds = new List<Guid> { invoiceTypeId };
            List<string> partnerIds = new List<string> { null, "", "gdsgfdsg", invoicesToSave[0].Invoice.PartnerId, invoicesToSave[1].Invoice.PartnerId, invoicesToSave[2].Invoice.PartnerId };
            List<int> invoiceIds = new List<int> { 0, 1, 10, 100, 1000 };
            UTUtilities.CallActionIteratively(TestGetLasInvoices, invoiceTypeIds, partnerIds, UTUtilities.GetNullableDateTimeListsForTesting(), invoiceIds);
        }

        void TestGetLasInvoices(Guid invoiceTypeId, string partnerId, DateTime? beforeDate, int lastInvoices)
        {
            var rdbResponse = _rdbDataManager.GetLasInvoices(invoiceTypeId, partnerId, beforeDate, lastInvoices);
            var sqlResponse = _sqlDataManager.GetLasInvoices(invoiceTypeId, partnerId, beforeDate, lastInvoices);

            AssertInvoicesAreSimilar(sqlResponse, rdbResponse);
        }

        void TestGetPartnerInvoicesByDate(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            List<Guid> invoiceTypeIds = new List<Guid> { invoiceTypeId };
            List<List<string>> partnerIdLists = new List<List<string>>
            {
                null,
                new List<string>(),
                new List<string> { "gdsgfdsg", "gerytert", invoicesToSave[0].Invoice.PartnerId },
                invoicesToSave.Select(itm => itm.Invoice.PartnerId).ToList()
            };
            List<DateTime> fromDates = new List<DateTime> { DateTime.Now, DateTime.Today, DateTime.Now.AddDays(-10), DateTime.Now.AddMonths(-4) };
            List<DateTime> toDates = new List<DateTime> { DateTime.Now, DateTime.Today, DateTime.Now.AddDays(10), DateTime.Now.AddMonths(4) };
            UTUtilities.CallActionIteratively(TestGetPartnerInvoicesByDate, invoiceTypeIds, partnerIdLists, fromDates, toDates);
        }

        void TestGetPartnerInvoicesByDate(Guid invoiceTypeId, List<string> partnerIds, DateTime fromDate, DateTime toDate)
        {
            var rdbResponse = _rdbDataManager.GetPartnerInvoicesByDate(invoiceTypeId, partnerIds, fromDate, toDate);
            var sqlResponse = _sqlDataManager.GetPartnerInvoicesByDate(invoiceTypeId, partnerIds, fromDate, toDate);

            AssertInvoicesAreSimilar(sqlResponse, rdbResponse);
        }

        void TestGetUnPaidPartnerInvoices(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestGetUnPaidPartnerInvoices(new List<PartnerInvoiceType>());
            TestGetUnPaidPartnerInvoices(new List<PartnerInvoiceType>
            {
                new PartnerInvoiceType
                {
                     InvoiceTypeId = invoiceTypeId,
                      PartnerId = ""
                },
                new PartnerInvoiceType
                {
                     InvoiceTypeId = invoiceTypeId,
                      PartnerId = "gdseryerytyterteruyterjh gfdsg"
                },
                new PartnerInvoiceType
                {
                     InvoiceTypeId = invoiceTypeId,
                      PartnerId = invoicesToSave[1].Invoice.PartnerId
                }
            });
            TestGetUnPaidPartnerInvoices(invoicesToSave.Select(itm => itm.Invoice.PartnerId).Distinct().Select(partnerId => new PartnerInvoiceType { InvoiceTypeId = invoiceTypeId, PartnerId = partnerId }).ToList());
        }

        void TestGetUnPaidPartnerInvoices(List<PartnerInvoiceType> input)
        {
            var rdbResponse = _rdbDataManager.GetUnPaidPartnerInvoices(input);
            if (rdbResponse != null)
                rdbResponse = rdbResponse.OrderBy(inv => inv.InvoiceId);
            var sqlResponse = _sqlDataManager.GetUnPaidPartnerInvoices(input);
            if (sqlResponse != null)
                sqlResponse = sqlResponse.OrderBy(inv => inv.InvoiceId);

            AssertInvoicesAreSimilar(sqlResponse, rdbResponse);
        }
        void TestLoadInvoicesAfterImportedId(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestLoadInvoicesAfterImportedId(invoiceTypeId, 0);
            TestLoadInvoicesAfterImportedId(invoiceTypeId, 10);
            TestLoadInvoicesAfterImportedId(invoiceTypeId, 100);
            TestLoadInvoicesAfterImportedId(invoiceTypeId, 1000);
            TestLoadInvoicesAfterImportedId(invoiceTypeId, invoicesToSave[0].Invoice.InvoiceId);
            TestLoadInvoicesAfterImportedId(invoiceTypeId, invoicesToSave[1].Invoice.InvoiceId);
        }

        void TestLoadInvoicesAfterImportedId(Guid invoiceTypeId, long lastImportedId)
        {
            List<Vanrise.Invoice.Entities.Invoice> rdbResponse = new List<Vanrise.Invoice.Entities.Invoice>();
            List<Vanrise.Invoice.Entities.Invoice> sqlResponse = new List<Vanrise.Invoice.Entities.Invoice>();

            _rdbDataManager.LoadInvoicesAfterImportedId(invoiceTypeId, lastImportedId, (inv) => rdbResponse.Add(inv));
            _sqlDataManager.LoadInvoicesAfterImportedId(invoiceTypeId, lastImportedId, (inv) => sqlResponse.Add(inv));

            AssertInvoicesAreSimilar(sqlResponse, rdbResponse);
        }

        void TestGetInvoiceBySourceId(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            TestGetInvoiceBySourceId(invoiceTypeId, (string)null);
            TestGetInvoiceBySourceId(invoiceTypeId, "");
            TestGetInvoiceBySourceId(invoiceTypeId, invoicesToSave.First(itm => itm.Invoice.SourceId != null).Invoice.SourceId);
            TestGetInvoiceBySourceId(invoiceTypeId, invoicesToSave.Last(itm => itm.Invoice.SourceId != null).Invoice.SourceId);
        }

        void TestGetInvoiceBySourceId(Guid invoiceTypeId, string sourceId)
        {
            var rdbResponse = _rdbDataManager.GetInvoiceBySourceId(invoiceTypeId, sourceId);
            var sqlResponse = _sqlDataManager.GetInvoiceBySourceId(invoiceTypeId, sourceId);

            AssertInvoiceIsSimilar(sqlResponse, rdbResponse);
        }

        void TestLoadInvoices(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            List<Guid> invoiceTypeIds = new List<Guid> { invoiceTypeId };
            List<DateTime?> froms = new List<DateTime?> { null, DateTime.Now, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-50), DateTime.Now.AddDays(-100), DateTime.Now.AddDays(-500) };
            List<DateTime?> tos = new List<DateTime?> { null, DateTime.Now, DateTime.Now.AddDays(10), DateTime.Now.AddDays(20), DateTime.Now.AddDays(50), DateTime.Now.AddDays(100), DateTime.Now.AddDays(500) };
            List<RecordFilterGroup> filterGroups = new List<RecordFilterGroup> { null };
            var filterGroup1 = new RecordFilterGroup { Filters = new List<RecordFilter>() };
            filterGroup1.Filters.Add(new StringRecordFilter { FieldName = "Partner", Value = invoicesToSave[0].Invoice.PartnerId, CompareOperator = StringRecordFilterOperator.Equals });
            filterGroups.Add(filterGroup1);
            var filterGroup2 = new RecordFilterGroup { Filters = new List<RecordFilter>() };
            filterGroup2.Filters.Add(new StringRecordFilter { FieldName = "Partner", Value = invoicesToSave[0].Invoice.PartnerId.Substring(0, 3), CompareOperator = StringRecordFilterOperator.StartsWith });
            filterGroups.Add(filterGroup2);
            List<OrderDirection?> orderDirections = new List<OrderDirection?> { null, OrderDirection.Ascending, OrderDirection.Descending };
            UTUtilities.CallActionIteratively(TestLoadInvoices, invoiceTypeIds, froms, tos, filterGroups, orderDirections);
        }

        void TestLoadInvoices(Guid invoiceTypeId, DateTime? from, DateTime? to, RecordFilterGroup filterGroup, OrderDirection? orderDirection)
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

            AssertInvoicesAreSimilar(sqlResponse, rdbResponse);
        }

        void TestCheckInvoiceOverlaping(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[1].Invoice;
            List<Guid> invoiceTypeIds = new List<Guid> { invoiceTypeId };
            List<string> partnerIds = new List<string> { null, "", "dsgfdsgfdrhgshhresat", invoice.PartnerId };
            List<DateTime> fromDates = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(-4), invoice.FromDate.AddDays(-20), invoice.FromDate.AddDays(-5), invoice.FromDate.AddDays(-2), invoice.FromDate, invoice.ToDate.AddDays(-3), invoice.ToDate.AddDays(-2), invoice.ToDate };
            List<DateTime> toDates = new List<DateTime> { DateTime.Now, invoice.FromDate.AddDays(-5), invoice.FromDate , invoice.FromDate.AddDays(1), invoice.ToDate.AddDays(-2), invoice.ToDate, invoice.ToDate.AddDays(5) };
            List<long?> invoiceIds = new List<long?> { null, invoice.InvoiceId, 3243256543 };
            UTUtilities.CallActionIteratively(TestCheckInvoiceOverlaping, invoiceTypeIds, partnerIds, fromDates, toDates, invoiceIds);
        }

        void TestCheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId)
        {
            var rdbResponse = _rdbDataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId, fromDate, toDate, invoiceId);
            var sqlResponse = _sqlDataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId, fromDate, toDate, invoiceId);

            UTAssert.ObjectsAreEqual(sqlResponse, rdbResponse);            
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

        void AssertInvoiceIsSimilar(Vanrise.Invoice.Entities.Invoice sqlInvoice, Vanrise.Invoice.Entities.Invoice rdbInvoice)
        {
            if (sqlInvoice == null && rdbInvoice == null)
                return;
            AssertInvoicesAreSimilar(new List<Vanrise.Invoice.Entities.Invoice> { sqlInvoice }, new List<Vanrise.Invoice.Entities.Invoice> { rdbInvoice });
        }

        void AssertInvoicesAreSimilar(IEnumerable<Vanrise.Invoice.Entities.Invoice> sqlInvoices, IEnumerable<Vanrise.Invoice.Entities.Invoice> rdbInvoices)
        {
            if (sqlInvoices == null && rdbInvoices == null)
                return;
            foreach (var invoice in sqlInvoices)
            {
                invoice.CreatedTime = default(DateTime);
            }
            foreach (var invoice in rdbInvoices)
            {
                invoice.CreatedTime = default(DateTime);
            }
            UTAssert.ObjectsAreSimilar(sqlInvoices, rdbInvoices);
        }

        private void AssertInvoiceItemsAreSimilar(IEnumerable<InvoiceItem> sqlInvoiceItems, IEnumerable<InvoiceItem> rdbInvoiceItems)
        {
            sqlInvoiceItems = sqlInvoiceItems.OrderBy(itm => itm.InvoiceItemId);
            rdbInvoiceItems = rdbInvoiceItems.OrderBy(itm => itm.InvoiceItemId);
            UTAssert.ObjectsAreSimilar(sqlInvoiceItems, rdbInvoiceItems);
        }

        #endregion

        #region Test Update

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

        void TestApproveInvoice(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[1].Invoice;
            TestApproveInvoice(invoice.InvoiceId, null, null, invoiceTypeId, invoicesToSave);
            TestApproveInvoice(invoice.InvoiceId, DateTime.Now, 7, invoiceTypeId, invoicesToSave);
            TestApproveInvoice(invoice.InvoiceId, null, 3, invoiceTypeId, invoicesToSave);
            TestApproveInvoice(invoice.InvoiceId, DateTime.Now, null, invoiceTypeId, invoicesToSave);
            TestApproveInvoice(invoice.InvoiceId, DateTime.Now, 2, invoiceTypeId, invoicesToSave);
        }

        void TestApproveInvoice(long invoiceId, DateTime? approveddDate, int? approvedBy, Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            _rdbDataManager.ApproveInvoice(invoiceId, approveddDate, approvedBy);
            _sqlDataManager.ApproveInvoice(invoiceId, approveddDate, approvedBy);

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

        void TestUpdateInvoiceSettings(Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            var invoice = invoicesToSave[1].Invoice;
            TestUpdateInvoiceSettings(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceSettings(invoice.InvoiceId, new InvoiceSettings { FileId = 5 }, invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceSettings(invoice.InvoiceId, null, invoiceTypeId, invoicesToSave);
            TestUpdateInvoiceSettings(invoice.InvoiceId, new InvoiceSettings { FileId = 534 }, invoiceTypeId, invoicesToSave);
        }

        void TestUpdateInvoiceSettings(long invoiceId, InvoiceSettings invoiceSettings, Guid invoiceTypeId, List<GenerateInvoiceInputToSave> invoicesToSave)
        {
            _rdbDataManager.UpdateInvoiceSettings(invoiceId, invoiceSettings);
            _sqlDataManager.UpdateInvoiceSettings(invoiceId, invoiceSettings);

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

        #endregion

        #endregion
    }
}