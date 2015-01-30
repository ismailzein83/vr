using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data;

namespace TOne.Business
{
    public class BusinessEntityManager
    {
        public List<TOne.Entities.CarrierInfo> GetCarriers()
        {
            ICarrierDataManager datamanager = DataManagerFactory.GetDataManager<ICarrierDataManager>();

            return datamanager.GetCarriers();
        }
    }
}
