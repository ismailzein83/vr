using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class TODManager
    {
        IToDConsiderationDataManager _dataManager;

        public TODManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IToDConsiderationDataManager>();
        }

        public Vanrise.Entities.IDataRetrievalResult<TODConsiderationInfo> GetFilteredCustomerTOD(Vanrise.Entities.DataRetrievalInput<TODCustomerQuery> input)
        {
            return null;
                //Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetCarrierMasksByCriteria(input));
        }
        
    }
}
