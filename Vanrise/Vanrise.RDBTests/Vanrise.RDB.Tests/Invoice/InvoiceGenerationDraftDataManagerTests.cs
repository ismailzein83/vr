using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class InvoiceGenerationDraftDataManagerTests
    {
        const string DBTABLE_NAME_INVOICEGENERATIONDRAFT = "InvoiceGenerationDraft";

        IInvoiceGenerationDraftDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();
        IInvoiceGenerationDraftDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();

        [TestMethod]
        public void InsertUpdateGetDeleteInvoiceGenerationDraft()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICEGENERATIONDRAFT);

            Guid draftUniqueIdentifier1 = Guid.NewGuid();
            InsertUpdateGetDeleteInvoiceGenerationDraft(draftUniqueIdentifier1);
            var draftUniqueIdentifier2 = Guid.NewGuid();
            InsertUpdateGetDeleteInvoiceGenerationDraft(draftUniqueIdentifier2);
            var draftUniqueIdentifier3 = Guid.NewGuid();
            InsertUpdateGetDeleteInvoiceGenerationDraft(draftUniqueIdentifier3);

            ClearInvoiceGenerationDrafts(draftUniqueIdentifier1);
            AssertAllDraftsAreSimilar();

            ClearInvoiceGenerationDrafts(draftUniqueIdentifier2);
            AssertAllDraftsAreSimilar();

            ClearInvoiceGenerationDrafts(draftUniqueIdentifier2);
            AssertAllDraftsAreSimilar();
        }

        private void InsertUpdateGetDeleteInvoiceGenerationDraft(Guid draftUniqueIdentifier)
        {
            List<InvoiceGenerationDraft> drafts = new List<InvoiceGenerationDraft>();
            drafts.Add(new InvoiceGenerationDraft
            {
                InvoiceTypeId = Guid.NewGuid(),
                InvoiceGenerationIdentifier = draftUniqueIdentifier,
                From = DateTime.Now.AddMonths(-1),
                To = DateTime.Now,
                PartnerId = Guid.NewGuid().ToString(),
                PartnerName = Guid.NewGuid().ToString(),
                CustomPayload = new DraftPayload { Prop = Guid.NewGuid().ToString() }
            });
            drafts.Add(new InvoiceGenerationDraft
            {
                InvoiceTypeId = Guid.NewGuid(),
                InvoiceGenerationIdentifier = draftUniqueIdentifier,
                From = DateTime.Now.AddMonths(-2),
                To = DateTime.Now,
                PartnerId = Guid.NewGuid().ToString(),
                PartnerName = Guid.NewGuid().ToString(),
                CustomPayload = new DraftPayload { Prop = Guid.NewGuid().ToString() }
            });
            drafts.Add(new InvoiceGenerationDraft
            {
                InvoiceTypeId = Guid.NewGuid(),
                InvoiceGenerationIdentifier = draftUniqueIdentifier,
                From = DateTime.Now.AddMonths(-5),
                To = DateTime.Now,
                PartnerId = Guid.NewGuid().ToString(),
                PartnerName = Guid.NewGuid().ToString(),
                CustomPayload = new DraftPayload { Prop = Guid.NewGuid().ToString() }
            });
            drafts.Add(new InvoiceGenerationDraft
            {
                InvoiceTypeId = Guid.NewGuid(),
                InvoiceGenerationIdentifier = draftUniqueIdentifier,
                From = DateTime.Now.AddMonths(-12),
                To = DateTime.Now.AddDays(-34),
                PartnerId = Guid.NewGuid().ToString(),
                PartnerName = Guid.NewGuid().ToString(),
                CustomPayload = new DraftPayload { Prop = Guid.NewGuid().ToString() }
            });
            drafts.Add(new InvoiceGenerationDraft
            {
                InvoiceTypeId = Guid.NewGuid(),
                InvoiceGenerationIdentifier = draftUniqueIdentifier,
                From = DateTime.Now.AddMonths(-42),
                To = DateTime.Now,
                PartnerId = Guid.NewGuid().ToString(),
                PartnerName = Guid.NewGuid().ToString(),
                CustomPayload = new DraftPayload { Prop = Guid.NewGuid().ToString() }
            });

            foreach (var draft in drafts)
            {
                long sqlId;
                long rdbId;
                UTAssert.ObjectsAreEqual(_sqlDataManager.InsertInvoiceGenerationDraft(draft, out sqlId), _rdbDataManager.InsertInvoiceGenerationDraft(draft, out rdbId));
                UTAssert.ObjectsAreEqual(sqlId, rdbId);
                draft.InvoiceGenerationDraftId = rdbId;
                AssertDraftsAreSimilar(draftUniqueIdentifier);
            }

            foreach(var draft in drafts)
            {
                UTAssert.ObjectsAreSimilar(_sqlDataManager.GetInvoiceGenerationDraft(draft.InvoiceGenerationDraftId), _rdbDataManager.GetInvoiceGenerationDraft(draft.InvoiceGenerationDraftId));
            }

            foreach (var draft in drafts)
            {
                var draftToEdit = new InvoiceGenerationDraftToEdit
                {
                    InvoiceGenerationDraftId = draft.InvoiceGenerationDraftId,
                    From = draft.From.AddDays(-4),
                    CustomPayload = new DraftPayload { Prop = Guid.NewGuid().ToString() },
                    IsSelected = true,
                    To = draft.To.AddDays(54)
                };
                UTAssert.ObjectsAreEqual(_sqlDataManager.UpdateInvoiceGenerationDraft(draftToEdit), _rdbDataManager.UpdateInvoiceGenerationDraft(draftToEdit));
                AssertDraftsAreSimilar(draftUniqueIdentifier);
            }

            _sqlDataManager.DeleteInvoiceGenerationDraft(drafts[1].InvoiceGenerationDraftId);
            _rdbDataManager.DeleteInvoiceGenerationDraft(drafts[1].InvoiceGenerationDraftId);
            AssertDraftsAreSimilar(draftUniqueIdentifier);
        }

        private InvoiceGenerationDraftToEdit BuildDraftToEdit(InvoiceGenerationDraft draft)
        {
            return new InvoiceGenerationDraftToEdit
            {
                InvoiceGenerationDraftId = draft.InvoiceGenerationDraftId,
                From = draft.From.AddDays(-3),
                To = draft.To.AddDays(20),
                CustomPayload = new DraftPayload { Prop = "f sodigp jiu erut " },
                IsSelected = true
            };
        }

        private void ClearInvoiceGenerationDrafts(Guid draftUniqueIdentifier)
        {
            _sqlDataManager.ClearInvoiceGenerationDrafts(draftUniqueIdentifier);
            _rdbDataManager.ClearInvoiceGenerationDrafts(draftUniqueIdentifier);
        }

        private void AssertDraftsAreSimilar(Guid draftUniqueIdentifier)
        {
            AssertAllDraftsAreSimilar();

            var sqlDrafts = _sqlDataManager.GetInvoiceGenerationDrafts(draftUniqueIdentifier);
            var rdbDrafts = _rdbDataManager.GetInvoiceGenerationDrafts(draftUniqueIdentifier);
            
            UTAssert.ObjectsAreSimilar(sqlDrafts, rdbDrafts);

            var sqlSummary = _sqlDataManager.GetInvoiceGenerationDraftsSummary(draftUniqueIdentifier);
            var rdbSummary = _rdbDataManager.GetInvoiceGenerationDraftsSummary(draftUniqueIdentifier);

            UTAssert.ObjectsAreSimilar(sqlSummary, rdbSummary);
        }

        private void AssertAllDraftsAreSimilar()
        {
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICEGENERATIONDRAFT);
        }

        private class DraftPayload
        {
            public string Prop { get; set; }
        }
    }
}
