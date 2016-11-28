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
        List<CarrierAccount> GetCarrierAccounts();
        bool Insert(CarrierAccount carrierAccount,out int carrierAccountId);
        bool Update(CarrierAccountToEdit carrierAccount, int carrierProfileId);
        bool AreCarrierAccountsUpdated(ref object updateHandle);
        bool UpdateExtendedSettings(int carrierAccountId, Dictionary<string, Object> extendedSettings);
    }
}
