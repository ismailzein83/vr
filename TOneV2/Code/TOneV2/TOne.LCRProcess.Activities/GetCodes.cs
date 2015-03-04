using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.BusinessEntity.Entities;
using Vanrise.Queueing;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;
using TOne.BusinessEntity.Business;

namespace TOne.LCRProcess.Activities
{

    public class GetCodesInput
    {
        public String CodePrefixGroup { get; set; }
        public DateTime EffectiveTime { get; set; }
        public bool IsFuture { get; set; }
        public List<CarrierAccountInfo> ActiveSuppliers { get; set; }
    }

    public class GetCodesOutput
    {
        public SuppliersCodes SuppliersCodes { get; set; }
        public List<String> DistinctCodes { get; set; }
        public HashSet<Int32> SaleZoneIds { get; set; }
        public HashSet<Int32> CostZoneIds { get; set; }
    }

    public sealed class GetCodes : BaseAsyncActivity<GetCodesInput, GetCodesOutput>
    {


        [RequiredArgument]
        public InArgument<DateTime> EffectiveTime { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<String> CodePrefixGroup { get; set; }

        [RequiredArgument]
        public InArgument<List<CarrierAccountInfo>> ActiveSuppliers { get; set; }

        [RequiredArgument]
        public OutArgument<List<String>> DistinctCodes { get; set; }

        [RequiredArgument]
        public OutArgument<SuppliersCodes> SupplierCodes { get; set; }

        [RequiredArgument]
        public OutArgument<HashSet<Int32>> SaleZoneIds { get; set; }

        [RequiredArgument]
        public OutArgument<HashSet<Int32>> CostZoneIds { get; set; }

        protected override GetCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCodesInput()
            {

                EffectiveTime = this.EffectiveTime.Get(context),
                IsFuture = this.IsFuture.Get(context),
                CodePrefixGroup = this.CodePrefixGroup.Get(context),
                ActiveSuppliers = this.ActiveSuppliers.Get(context)
            };
        }

        protected override GetCodesOutput DoWorkWithResult(GetCodesInput inputArgument, AsyncActivityHandle handle)
        {
            CodeManager codeManager = new CodeManager();
            List<string> distinctCodes;
            HashSet<Int32> saleZoneIds;
            HashSet<Int32> supplierZoneIds;
            SuppliersCodes suppliersCodes = codeManager.GetCodesByCodePrefixGroup(inputArgument.CodePrefixGroup, inputArgument.EffectiveTime, inputArgument.IsFuture, inputArgument.ActiveSuppliers, out distinctCodes, out supplierZoneIds, out saleZoneIds);
            return new GetCodesOutput
            {
                DistinctCodes = distinctCodes,
                SuppliersCodes = suppliersCodes,
                CostZoneIds = supplierZoneIds,
                SaleZoneIds = saleZoneIds
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetCodesOutput result)
        {
            this.DistinctCodes.Set(context, result.DistinctCodes);
            this.SupplierCodes.Set(context, result.SuppliersCodes);
            this.SaleZoneIds.Set(context, result.SaleZoneIds);
            this.CostZoneIds.Set(context, result.CostZoneIds);
        }
    }
}
