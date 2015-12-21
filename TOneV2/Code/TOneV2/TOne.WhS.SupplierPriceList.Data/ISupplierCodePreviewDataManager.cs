using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierCodePreviewDataManager : IDataManager
    {
        void Insert(int priceListId, IEnumerable<CodePreview> codePreviewList);
        Vanrise.Entities.BigResult<Entities.CodePreview> GetCodePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input);
    }
}
