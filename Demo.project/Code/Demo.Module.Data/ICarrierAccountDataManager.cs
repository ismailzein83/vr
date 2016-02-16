using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;


namespace Demo.Module.Data
{
    public interface ICarrierAccountDataManager:IDataManager
    {
        List<CarrierAccount> GetCarrierAccounts();
        bool Insert(CarrierAccount carrierAccount,out int carrierAccountId);
        bool Update(CarrierAccount carrierAccount);
        bool AreCarrierAccountsUpdated(ref object updateHandle);
    }
}
