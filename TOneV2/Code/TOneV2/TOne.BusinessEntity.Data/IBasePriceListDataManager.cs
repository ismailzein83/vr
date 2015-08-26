using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IBasePriceListDataManager:IDataManager
    {
        Vanrise.Entities.BigResult<T> GetPriceListDetails<T>(Vanrise.Entities.DataRetrievalInput<int> input) where T : BasePriceListDetail;
    }
}
