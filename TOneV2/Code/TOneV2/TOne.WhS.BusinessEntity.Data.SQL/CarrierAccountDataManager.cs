using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CarrierAccountDataManager:BaseSQLDataManager,ICarrierAccountDataManager
    {
        public CarrierAccountDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public List<CarrierAccountInfo> GetCarrierAccounts(bool getCustomers, bool getSuppliers)
        {
            List<int> carriersTypeList = new List<int>();

            if (getCustomers)
            {
                if (!carriersTypeList.Contains(1))
                    carriersTypeList.Add(1);
                carriersTypeList.Add(3);    
            }
            if (getSuppliers)
            {
                if (!carriersTypeList.Contains(1))
                    carriersTypeList.Add(1);
                carriersTypeList.Add(2);
            } 
            string carrierTypes = null;
            if (carriersTypeList.Count > 0)
                carrierTypes = string.Join<int>(",", carriersTypeList);
            return GetItemsSP("TOneWhS_BE.sp_CarrierAccount_GetFiltered", CarrierAccountInfoMapper, carrierTypes);
        }
        private CarrierAccountInfo CarrierAccountInfoMapper(IDataReader reader)
        {
            CarrierAccountInfo carrierAccountInfo = new CarrierAccountInfo
            {
                CarrierAccountId = (int)reader["ID"],
                Name = reader["Name"] as string
            };
            return carrierAccountInfo;
        }
    }
}
