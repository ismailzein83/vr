using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPricelistPreviewSummaryManager
    {
        public PreviewSummary GetSupplierPricelistPreviewSummary(int processInstanceId)
        {
            ISupplierPricelistPreviewSummaryDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPricelistPreviewSummaryDataManager>();
            return dataManager.GetSupplierPricelistPreviewSummary(processInstanceId);
        }
    }
}
