using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class BasePricelistManager<T> where T : TOne.BusinessEntity.Entities.BasePriceListDetail
    {
       public  Vanrise.Entities.IDataRetrievalResult<T> GetPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input){
         
           IBasePriceListDataManager dataManager=BEDataManagerFactory.GetDataManager<IBasePriceListDataManager>();

           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetPriceListDetails<T>(input));

       }
    }
}
