﻿using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ISalePricelistFileContext
    {
        int SellingNumberPlanId { get; }
        long ProcessInstanceId { get; set; }
        IEnumerable<int> CustomerIds { get; }
        IEnumerable<SalePLZoneChange> ZoneChanges { get; }
        DateTime EffectiveDate { get; }
        SalePLChangeType ChangeType { get; }
        IEnumerable<int> EndedCountryIds { get; }
        DateTime? CountriesEndedOn { get; }
        IEnumerable<NewCustomerPriceListChange> CustomerPriceListChanges { get; set; }
        IEnumerable<NewPriceList> SalePriceLists { get; }
        int CurrencyId { get; }
        int UserId { get; }

        void WriteMessageToWorkflowLogs(string messageFormat, params object[] args);
    }
}
