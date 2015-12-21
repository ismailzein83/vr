using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierCodePreviewManager
    {
        public void Insert(int priceListId, IEnumerable<CodePreview> codePreviewList)
        {
            ISupplierCodePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodePreviewDataManager>();
            dataManager.Insert(priceListId, codePreviewList);
        }
        public Vanrise.Entities.IDataRetrievalResult<CodePreview> GetFilteredCodePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ISupplierCodePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodePreviewDataManager>();
            return dataManager.GetCodePreviewFilteredFromTemp(input);
        }
    }
}
