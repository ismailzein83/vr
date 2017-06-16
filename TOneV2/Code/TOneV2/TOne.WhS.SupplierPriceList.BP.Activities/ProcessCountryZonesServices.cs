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

    public class ProcessCountryZonesServicesInput
    {
        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public int SupplierId { get; set; }

        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }

        public IEnumerable<ExistingZoneService> ExistingZonesServices { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public DateTime PriceListDate { get; set; }

        public IEnumerable<int> ImportedServiceTypeIds { get; set; }

        public IEnumerable<NotImportedZone> NotImportedZones { get; set; }

        public bool IncludeServices { get; set; }
        public DateTime MinimumDate { get; set; }

    }

    public class ProcessCountryZonesServicesOutput
    {
        public IEnumerable<NewZoneService> NewZonesServices { get; set; }

        public IEnumerable<ChangedZoneService> ChangedZonesServices { get; set; }

    }

    #endregion

    public sealed class ProcessCountryZonesServices : BaseAsyncActivity<ProcessCountryZonesServicesInput, ProcessCountryZonesServicesOutput>
    {

        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZoneService>> ExistingZonesServices { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> PriceListDate { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<int>> ImportedServiceTypeIds { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<bool> IncludeServices { get; set; }

        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<NewZoneService>> NewZonesServices { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedZoneService>> ChangedZonesServices { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            handle.CustomData.Add(ImportSPLContext.CustomDataKey, context.GetSPLParameterContext());
            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessCountryZonesServicesOutput DoWorkWithResult(ProcessCountryZonesServicesInput inputArgument, AsyncActivityHandle handle)
        {
            PriceListZoneServiceManager manager = new PriceListZoneServiceManager();
            manager.ProcessRetroActiveZoneServices(inputArgument.SupplierId, inputArgument.MinimumDate);

            IEnumerable<ExistingZone> existingZones = null;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            if (inputArgument.IncludeServices == false)
                return new ProcessCountryZonesServicesOutput()
                {
                    ChangedZonesServices = new List<ChangedZoneService>(),
                    NewZonesServices = new List<NewZoneService>()
                };

            ProcessCountryZonesServicesContext processCountryZonesServicesContext = new ProcessCountryZonesServicesContext()
            {
                ImportedZones = inputArgument.ImportedZones,
                ExistingZones = existingZones,
                NewAndExistingZones = inputArgument.NewAndExistingZones,
                PriceListDate = inputArgument.PriceListDate,
                NotImportedZones = inputArgument.NotImportedZones,
                ExistingZonesServices = inputArgument.ExistingZonesServices,
                MinimumDate = inputArgument.MinimumDate
            };

            
            manager.ProcessCountryZonesServices(processCountryZonesServicesContext, inputArgument.ImportedServiceTypeIds, inputArgument.SupplierId);

            if ((processCountryZonesServicesContext.NewZonesServices != null && processCountryZonesServicesContext.NewZonesServices.Count() > 0)
                || (processCountryZonesServicesContext.ChangedZonesServices != null && processCountryZonesServicesContext.ChangedZonesServices.Count() > 0))
            {
                IImportSPLContext splContext = handle.CustomData[ImportSPLContext.CustomDataKey] as IImportSPLContext;
                splContext.SetToTrueProcessHasChangesWithLock();
            }

            return new ProcessCountryZonesServicesOutput()
            {
                ChangedZonesServices = processCountryZonesServicesContext.ChangedZonesServices,
                NewZonesServices = processCountryZonesServicesContext.NewZonesServices
            };
        }

        protected override ProcessCountryZonesServicesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryZonesServicesInput()
            {
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context),
                SupplierId = this.SupplierId.Get(context),
                ImportedZones = this.ImportedZones.Get(context),
                NewAndExistingZones = this.NewAndExistingZones.Get(context),
                PriceListDate = this.PriceListDate.Get(context),
                ImportedServiceTypeIds = this.ImportedServiceTypeIds.Get(context),
                NotImportedZones = this.NotImportedZones.Get(context),
                ExistingZonesServices = this.ExistingZonesServices.Get(context),
                IncludeServices = this.IncludeServices.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryZonesServicesOutput result)
        {
            NewZonesServices.Set(context, result.NewZonesServices);
            ChangedZonesServices.Set(context, result.ChangedZonesServices);
        }
    }
}
