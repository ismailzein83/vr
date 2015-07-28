using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.BusinessEntity.Entities;
using TOne.BusinessEntity.Data;

namespace TOne.BusinessEntity.BProcess.Activities
{

    public sealed class GetActiveSuppliersInfo : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<List<CarrierAccountInfo>> ActiveSuppliers { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            var activeSuppliers = dataManager.GetActiveSuppliersInfo();
            this.ActiveSuppliers.Set(context, activeSuppliers);
        }
    }
}
