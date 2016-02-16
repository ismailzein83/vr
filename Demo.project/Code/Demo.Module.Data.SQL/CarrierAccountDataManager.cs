using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class CarrierAccountDataManager : BaseSQLDataManager, ICarrierAccountDataManager
    {

        #region ctor/Local Variables
        public CarrierAccountDataManager(): base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {
        }
        #endregion

        #region Public Methods
        public bool Insert(CarrierAccount carrierAccount, out int insertedId)
        {

            object carrierAccountId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_CarrierAccount_Insert", out carrierAccountId, carrierAccount.NameSuffix, carrierAccount.CarrierProfileId, carrierAccount.AccountType, carrierAccount.SellingNumberPlanId, Vanrise.Common.Serializer.Serialize(carrierAccount.CustomerSettings),
                Vanrise.Common.Serializer.Serialize(carrierAccount.SupplierSettings), Vanrise.Common.Serializer.Serialize(carrierAccount.CarrierAccountSettings));
            insertedId = (int)carrierAccountId;
            return (recordsEffected > 0);
        }
        public bool Update(CarrierAccount carrierAccount)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_CarrierAccount_Update", carrierAccount.CarrierAccountId, carrierAccount.NameSuffix, carrierAccount.CarrierProfileId, carrierAccount.AccountType, Vanrise.Common.Serializer.Serialize(carrierAccount.CustomerSettings),
                 Vanrise.Common.Serializer.Serialize(carrierAccount.SupplierSettings), Vanrise.Common.Serializer.Serialize(carrierAccount.CarrierAccountSettings));
            return (recordsEffected > 0);
        }
        public List<CarrierAccount> GetCarrierAccounts()
        {
            return GetItemsSP("dbo.sp_CarrierAccount_GetAll", CarrierAccountMapper);
        }
        public bool AreCarrierAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.CarrierAccount", ref updateHandle);
        }
        #endregion


        #region  Mappers
        private CarrierAccount CarrierAccountMapper(IDataReader reader)
        {
            CarrierAccount carrierAccount = new CarrierAccount
            {
                CarrierAccountId = (int)reader["ID"],
                NameSuffix = reader["NameSuffix"] as string,
                AccountType = (CarrierAccountType)GetReaderValue<int>(reader, "AccountType"),
                SupplierSettings = Vanrise.Common.Serializer.Deserialize<Entities.CarrierAccountSupplierSettings>(reader["SupplierSettings"] as string),
                CustomerSettings = Vanrise.Common.Serializer.Deserialize<Entities.CarrierAccountCustomerSettings>(reader["CustomerSettings"] as string),
                SellingNumberPlanId = GetReaderValue<int?>(reader, "SellingNumberPlanID"),
                CarrierProfileId = (int)reader["CarrierProfileId"],
                CarrierAccountSettings = Vanrise.Common.Serializer.Deserialize<Entities.CarrierAccountSettings>(reader["CarrierAccountSettings"] as string),

            };
            return carrierAccount;
        }
        #endregion

    }
}
