using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Entities;
using TABS;
using Vanrise.BusinessProcess;
using TOne.LCR.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class GetSupplierCodesInput
    {
        public string SupplierID { get; set; }
        public bool IsFuture { get; set; }
    }

    public class GetSupplierCodesOutput
    {
        public List<LCRCode> SupplierCodes { get; set; }
    }

    #endregion

    public sealed class GetSupplierCodes : BaseAsyncActivity<GetSupplierCodesInput, GetSupplierCodesOutput>
    {
        [RequiredArgument]
        public InArgument<string> SupplierID { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<List<LCRCode>> SupplierCodes { get; set; }

        protected override GetSupplierCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetSupplierCodesInput
            {
                SupplierID = this.SupplierID.Get(context),
                IsFuture = this.IsFuture.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetSupplierCodesOutput result)
        {
            this.SupplierCodes.Set(context, result.SupplierCodes);
        }

        protected override GetSupplierCodesOutput DoWorkWithResult(GetSupplierCodesInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime effectiveOn = DateTime.Today;
            if (inputArgument.IsFuture)
            {
                var noticeDays = (double)TABS.SystemConfiguration.KnownParameters[KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value;
                effectiveOn = DateTime.Today.AddDays(noticeDays);
            }
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            List<LCRCode> supplierCodes = dataManager.GetSupplierCodes(inputArgument.SupplierID, effectiveOn);
            return new GetSupplierCodesOutput
            {
                SupplierCodes = supplierCodes
            };
        }
    }
}
