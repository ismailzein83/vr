using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class BaseTODManager<T> where T : TOne.BusinessEntity.Entities.BaseTODConsiderationInfo
    {
        public Vanrise.Entities.IDataRetrievalResult<T> GetTODinfos(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {

           IBaseTODDataManager dataManager = BEDataManagerFactory.GetDataManager<IBaseTODDataManager>();

           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetTODinfos<T>(input));

       }
    }
}
