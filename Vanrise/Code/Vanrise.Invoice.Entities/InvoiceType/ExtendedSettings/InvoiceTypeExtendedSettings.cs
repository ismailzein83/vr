﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoiceTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual string GenerationCustomSectionDirective { get; set; }
        public abstract InvoiceGenerator GetInvoiceGenerator();
        public abstract InvoicePartnerManager GetPartnerManager();
        public abstract dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context);
        public abstract void GetInitialPeriodInfo(IInitialPeriodInfoContext context);
        public abstract IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context);
    }

    public abstract class InvoiceGenerator
    {
        public abstract void GenerateInvoice(IInvoiceGenerationContext context);
    }
    public interface IInvoiceGenerationContext
    {
        Guid InvoiceTypeId { get; }
        string PartnerId { get; }
        DateTime IssueDate { get; }
        DateTime FromDate { get; }
        DateTime ToDate { get; }
        DateTime GeneratedToDate { get; }
        dynamic CustomSectionPayload { get; }
        int GetDuePeriod();
        GeneratedInvoice Invoice { set; }
        string ErrorMessage { set; }
        List<GeneratedInvoiceBillingTransaction> BillingTransactions { set; }
    }
    public interface IInvoiceTypeExtendedSettingsInfoContext
    {
        string InfoType { get; set; }
        Entities.Invoice Invoice { get; set; }
    }

    public interface IInitialPeriodInfoContext
    {
        string PartnerId { get; }
        DateTime InitialStartDate { set; }
        DateTime PartnerCreationDate { set; }
    }
    public interface IExtendedSettingsPartnerIdsContext
    {
        Guid InvoiceTypeId { get; }
        PartnerRetrievalType PartnerRetrievalType { get; }
    }
    public enum PartnerRetrievalType { GetAll = 1, GetInactive = 2, GetActive = 3 }
}
