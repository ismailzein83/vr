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
                CarrierProfileId = (int)reader["CarrierProfileId"],
                 
            };
            carrierAccountDetail.AccountTypeDescription = carrierAccountDetail.AccountType.ToString();
            return carrierAccountDetail;
        }


        public bool Insert(CarrierAccount carrierAccount, out int insertedId)
        {

            object carrierAccountId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierAccount_Insert", out carrierAccountId, carrierAccount.Name,carrierAccount.CarrierProfileId,carrierAccount.AccountType, Vanrise.Common.Serializer.Serialize(carrierAccount.CustomerSettings),
                Vanrise.Common.Serializer.Serialize(carrierAccount.SupplierSettings));
            insertedId = (int)carrierAccountId;
            return (recordsEffected > 0);
        }


        public bool Update(CarrierAccount carrierAccount)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierAccount_Update", carrierAccount.CarrierAccountId, carrierAccount.Name, carrierAccount.CarrierProfileId, carrierAccount.AccountType, Vanrise.Common.Serializer.Serialize(carrierAccount.CustomerSettings),
                 Vanrise.Common.Serializer.Serialize(carrierAccount.SupplierSettings));
            return (recordsEffected > 0);
        }
        public List<CarrierAccountDetail> GetCarrierAccounts()
        {
            return GetItemsSP("TOneWhS_BE.sp_CarrierAccount_GetAll", CarrierAccountDetailMapper);
        }


        public bool AreCarrierAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CarrierAccount", ref updateHandle);
        }
    }
}
