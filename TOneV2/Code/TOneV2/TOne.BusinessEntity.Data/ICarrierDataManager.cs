using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICarrierDataManager : IDataManager
    {
        List<CarrierAccountInfo> GetActiveSuppliersInfo();

        List<CarrierInfo> GetCarriers(CarrierType carrierType);
        List<CarrierAccount> GetCarrierAccounts(string name, string companyName, int from, int to);
        CarrierAccount GetCarrierAccount(string carrierAccountId);

        int InsertCarrierTest(string CarrierAccountID, string Name);

        string GetCarrierAccountName(string carrierAccountId);
        int UpdateCarrierAccount(CarrierAccount carrierAccount);

        Dictionary<string, CarrierAccount> GetAllCarrierAccounts();

        Dictionary<int, CarrierGroup> GetAllCarrierGroups();
    }
}
