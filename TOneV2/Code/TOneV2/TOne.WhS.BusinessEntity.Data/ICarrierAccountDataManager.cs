using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;


namespace TOne.WhS.BusinessEntity.Data
{
    public interface ICarrierAccountDataManager:IDataManager
    {
        List<CarrierAccountInfo> GetCarrierAccounts(bool getCustomers, bool getSuppliers);
        Vanrise.Entities.BigResult<CarrierAccountDetail> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input);
        CarrierAccountDetail GetCarrierAccount(int carrierAccountId);

    }
}
