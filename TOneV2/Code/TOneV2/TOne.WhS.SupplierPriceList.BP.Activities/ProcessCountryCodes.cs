using System;
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

    public class ProcessCountryCodesInput
    {
        public SupplierPriceListType SupplierPriceListType { get; set; }

        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public IEnumerable<ImportedCode> ImportedCodes { get; set; }

        public IEnumerable<ExistingCode> ExistingCodes { get; set; }

        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }

        public DateTime PriceListDate { get; set; }

    }

    public class ProcessCountryCodesOutput
    {
        public ZonesByName NewAndExistingZones { get; set; }

        public IEnumerable<NewZone> NewZones { get; set; }

        public IEnumerable<ChangedZone> ChangedZones { get; set; }

        public IEnumerable<NewCode> NewCodes { get; set; }

        public IEnumerable<ChangedCode> ChangedCodes { get; set; }

    }

    #endregion

    public sealed class ProcessCountryCodes : BaseAsyncActivity<ProcessCountryCodesInput, ProcessCountryCodesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SupplierPriceListType> SupplierPriceListType { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> PriceListDate { get; set; }
        
        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NewZone>> NewZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedZone>> ChangedZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NewCode>> NewCodes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedCode>> ChangedCodes { get; set; }
        
        #endregion

        const string ImportSPLContext_CustomeDataKey = "ImportSPLContext";

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            handle.CustomData.Add(ImportSPLContext_CustomeDataKey, context.GetSPLParameterContext());
            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessCountryCodesOutput DoWorkWithResult(ProcessCountryCodesInput inputArgument, AsyncActivityHandle handle)
        {
            IImportSPLContext splContext = handle.CustomData[ImportSPLContext_CustomeDataKey] as IImportSPLContext;
            IEnumerable<ExistingZone> existingZones = null;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            ProcessCountryCodesContext processCountryCodesContext = new ProcessCountryCodesContext()
            {
                SupplierPriceListType = inputArgument.SupplierPriceListType,
                ImportedZones = inputArgument.ImportedZones,
                ImportedCodes = inputArgument.ImportedCodes,
                ExistingCodes = inputArgument.ExistingCodes,
                ExistingZones = existingZones,
                DeletedCodesDate = inputArgument.PriceListDate.Add(splContext.CodeCloseDateOffset),
                PriceListDate = inputArgument.PriceListDate
            };

            PriceListCodeManager plCodeManager = new PriceListCodeManager();
            plCodeManager.ProcessCountryCodes(processCountryCodesContext);

            return new ProcessCountryCodesOutput()
            {
                ChangedCodes = processCountryCodesContext.ChangedCodes,
                ChangedZones = processCountryCodesContext.ChangedZones,
                NewAndExistingZones = processCountryCodesContext.NewAndExistingZones,
                NewCodes = processCountryCodesContext.NewCodes,
                NewZones = processCountryCodesContext.NewZones
            };
        }

        protected override ProcessCountryCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryCodesInput()
            {
                SupplierPriceListType = this.SupplierPriceListType.Get(context),
                ImportedZones = this.ImportedZones.Get(context),
                PriceListDate = this.PriceListDate.Get(context),
                ExistingCodes = this.ExistingCodes.Get(context),
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context),
                ImportedCodes = this.ImportedCodes.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryCodesOutput result)
        {
            this.NewAndExistingZones.Set(context, result.NewAndExistingZones);
            this.NewZones.Set(context, result.NewZones);
            this.ChangedZones.Set(context, result.ChangedZones);
            this.NewCodes.Set(context, result.NewCodes);
            this.ChangedCodes.Set(context, result.ChangedCodes);
        }
    }
}
