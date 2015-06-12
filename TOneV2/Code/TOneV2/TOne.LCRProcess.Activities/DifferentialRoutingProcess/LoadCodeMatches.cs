using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class LoadCodeMatches : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<string>> TargetCodes { get; set; }

        [RequiredArgument]
        public InArgument<Int32> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InOutArgument<CodeMatchesByCode> CodeMatchesByCode { get; set; }

        [RequiredArgument]
        public InOutArgument<HashSet<int>> SupplierZoneIds { get; set; }

        [RequiredArgument]
        public InOutArgument<HashSet<int>> CustomerZoneIds { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            dataManager.DatabaseId = this.RoutingDatabaseId.Get(context);
            HashSet<int> supplierZoneIds = new HashSet<int>();
            HashSet<int> customerZoneIds = new HashSet<int>();
            CodeMatchesByCode codeMatches = dataManager.GetCodeMatchesByCodes(TargetCodes.Get(context), out supplierZoneIds, out customerZoneIds);
            this.CodeMatchesByCode.Set(context, codeMatches);
            this.SupplierZoneIds.Set(context, supplierZoneIds);
            this.CustomerZoneIds.Set(context, customerZoneIds);
        }
    }
}
