using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceDataManager : IDataManager
    {
        List<Entities.Invoice> GetPartnerInvoicesByDate(Guid invoiceTypeId, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate);
        IEnumerable<Entities.Invoice> GetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input);
        int GetInvoiceCount(Guid InvoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate);
        bool SaveInvoices(List<GenerateInvoiceInputToSave> generateInvoicesInputToSave, out List<long> insertedInvoiceIds);
        List<Entities.Invoice> GetInvoices(List<long> invoiceIds);

        bool CheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId);
        bool SetInvoicePaid(long invoiceId, DateTime? paidDate);
        bool SetInvoiceLocked(long invoiceId, DateTime? lockedDate);
        bool UpdateInvoiceNote(long invoiceId, string invoiceNote);
        void LoadInvoicesAfterImportedId(Guid invoiceTypeId, long lastImportedId, Action<Entities.Invoice> onInvoiceReady);
        IEnumerable<Entities.Invoice> GetUnPaidPartnerInvoices(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes);
        IEnumerable<InvoiceByPartnerInfo> GetLastInvoicesByPartners(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes);

        bool Update(Entities.Invoice invoice);
        Entities.Invoice GetInvoiceBySourceId(Guid invoiceTypeId, string sourceId);
        Entities.Invoice GetLastInvoice(Guid invoiceTypeId, string partnerId);
        bool UpdateInvoicePaidDateBySourceId(Guid invoiceTypeId, string sourceId, DateTime paidDate);
        IEnumerable<Entities.Invoice> GetLasInvoices(Guid invoiceTypeId, string partnerId, DateTime? beforeDate, int lastInvoices);
        VRPopulatedPeriod GetInvoicesPopulatedPeriod(Guid invoiceTypeId, string partnerId);
        bool CheckPartnerIfHasInvoices(Guid invoiceTypeId, string partnerId);
        List<Entities.Invoice> GetInvoicesBySerialNumbers(Guid invoiceTypeId, IEnumerable<string> serialNumbers);
        bool UpdateInvoicePaidDateById(Guid invoiceTypeId, long invoiceId, DateTime paidDate);
        bool UpdateInvoiceSettings(long invoiceId, InvoiceSettings invoiceSettings);
        bool SetInvoiceSentDate(long invoiceId, DateTime? sentDate);
        bool DeleteGeneratedInvoice(long invoiceId, Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate);
        bool ApproveInvoice(long invoiceId, DateTime? ApprovedDate, int? ApprovedBy);

        void LoadInvoices(Guid invoiceTypeId, DateTime? from, DateTime? to, RecordFilterGroup filterGroup, OrderDirection? orderDirection, Func<bool> shouldStop, Action<Entities.Invoice> onInvoiceLoaded);
    }
}
