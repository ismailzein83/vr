﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    #region Public Classes 

    public class ProcessCountryRatesInput
    {
        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public IEnumerable<ExistingRate> ExistingRates { get; set; }

        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public DateTime PriceListDate { get; set; }

        public IEnumerable<int> ImportedRateTypeIds { get; set; }

        public IEnumerable<NotImportedZone> NotImportedZones { get; set; }

    }

    public class ProcessCountryRatesOutput
    {
        public IEnumerable<NewRate> NewRates { get; set; }

        public IEnumerable<ChangedRate> ChangedRates { get; set; }

    }

    #endregion

    public sealed class ProcessCountryRates : BaseAsyncActivity<ProcessCountryRatesInput, ProcessCountryRatesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> PriceListDate { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<int>> ImportedRateTypeIds { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<NewRate>> NewRates { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            handle.CustomData.Add(ImportSPLContext.CustomDataKey, context.GetSPLParameterContext());
            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessCountryRatesOutput DoWorkWithResult(ProcessCountryRatesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ExistingZone> existingZones = null;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            ProcessCountryRatesContext processCountryRateContext = new ProcessCountryRatesContext()
            {
                ImportedZones = inputArgument.ImportedZones,
                ExistingRates = inputArgument.ExistingRates,
                ExistingZones = existingZones,
                NewAndExistingZones = inputArgument.NewAndExistingZones,
                PriceListDate = inputArgument.PriceListDate,
                NotImportedZones = inputArgument.NotImportedZones
            };

            PriceListRateManager manager = new PriceListRateManager();
            manager.ProcessCountryRates(processCountryRateContext, inputArgument.ImportedRateTypeIds);

            if((processCountryRateContext.NewRates != null && processCountryRateContext.NewRates.Count() > 0)
                || (processCountryRateContext.ChangedRates != null && processCountryRateContext.ChangedRates.Count() > 0))
            {
                IImportSPLContext splContext = handle.CustomData[ImportSPLContext.CustomDataKey] as IImportSPLContext;
                splContext.SetToTrueProcessHasChangesWithLock();
            }

            return new ProcessCountryRatesOutput()
            {
                ChangedRates = processCountryRateContext.ChangedRates,
                NewRates = processCountryRateContext.NewRates
            };
        }

        protected override ProcessCountryRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryRatesInput()
            {
                ExistingRates = this.ExistingRates.Get(context),
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context),
                ImportedZones = this.ImportedZones.Get(context),
                NewAndExistingZones = this.NewAndExistingZones.Get(context),
                ImportedRateTypeIds = this.ImportedRateTypeIds.Get(context),
                PriceListDate = this.PriceListDate.Get(context),
                NotImportedZones = this.NotImportedZones.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryRatesOutput result)
        {
            NewRates.Set(context, result.NewRates);
            ChangedRates.Set(context, result.ChangedRates);
        }
    }
}
