using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetUpdatedCodesSuppliers : CodeActivity
    {
        [RequiredArgument]
        public InArgument<byte[]> UpdatedAfter { get; set; }

        public OutArgument<List<string>> UpdatedSuppliers { get; set; }

        public OutArgument<byte[]> NewLastTimestamp { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            byte[] updatedAfter = this.UpdatedAfter.Get(context);
            if (updatedAfter == null)
                updatedAfter = new byte[0];
            byte[] newLastTimestamp;
            List<string> updatedSuppliers = dataManager.GetUpdatedCodesSuppliers(updatedAfter, out newLastTimestamp);
            this.UpdatedSuppliers.Set(context, updatedSuppliers);
            this.NewLastTimestamp.Set(context, newLastTimestamp);
        }
    }
}
