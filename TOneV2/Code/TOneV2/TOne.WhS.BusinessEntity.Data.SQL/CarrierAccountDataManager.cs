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
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static CarrierAccountDataManager()
        {
            _columnMapper.Add("CarrierAccountId", "ID");
        }

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

        private CarrierAccountDetail CarrierAccountDetailMapper(IDataReader reader)
        {
            CarrierAccountDetail carrierAccountDetail = new CarrierAccountDetail
            {
                CarrierAccountId = (int)reader["ID"],
                Name = reader["Name"] as string,
                AccountType = (CarrierAccountType)GetReaderValue<int>(reader,"AccountType"),
                SupplierSettings=Vanrise.Common.Serializer.Deserialize<Entities.CarrierAccountSupplierSettings>(reader["SupplierSettings"] as string),
                CarrierProfileName=reader["CarrierProfileName"] as string,
                CustomerSettings=Vanrise.Common.Serializer.Deserialize<Entities.CarrierAccountCustomerSettings>(reader["CustomerSettings"] as string),
                 
            };
            carrierAccountDetail.AccountTypeDescription = carrierAccountDetail.AccountType.ToString();
            return carrierAccountDetail;
        }

        public Vanrise.Entities.BigResult<CarrierAccountDetail> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                //string saleZonePackageIdsParam = null;
                //if (input.Query.SaleZonePackageIds != null)
                //    saleZonePackageIdsParam = string.Join(",", input.Query.SaleZonePackageIds);

                ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierAccount_CreateTempByFiltered", tempTableName, input.Query.CarrierAccountId, input.Query.AccountType, input.Query.Name);
            };

            return RetrieveData(input, createTempTableAction, CarrierAccountDetailMapper, _columnMapper);
        }


        public CarrierAccountDetail GetCarrierAccount(int carrierAccountId)
        {
            return GetItemSP("TOneWhS_BE.sp_CarrierAccount_Get", CarrierAccountDetailMapper, carrierAccountId);
        }
    }
}
