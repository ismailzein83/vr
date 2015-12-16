﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CarrierAccountDataManager : BaseSQLDataManager, ICarrierAccountDataManager
    {

        #region ctor/Local Variables
        public CarrierAccountDataManager(): base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public bool Insert(CarrierAccount carrierAccount, out int insertedId)
        {

            object carrierAccountId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierAccount_Insert", out carrierAccountId, carrierAccount.Name, carrierAccount.CarrierProfileId, carrierAccount.AccountType, carrierAccount.SellingNumberPlanId, Vanrise.Common.Serializer.Serialize(carrierAccount.CustomerSettings),
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
        public List<CarrierAccount> GetCarrierAccounts()
        {
            return GetItemsSP("TOneWhS_BE.sp_CarrierAccount_GetAll", CarrierAccountMapper);
        }
        public bool AreCarrierAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CarrierAccount", ref updateHandle);
        }
        #endregion

        #region Private Methods
        private CarrierAccount CarrierAccountMapper(IDataReader reader)
        {
            CarrierAccount carrierAccount = new CarrierAccount
            {
                CarrierAccountId = (int)reader["ID"],
                Name = reader["Name"] as string,
                AccountType = (CarrierAccountType)GetReaderValue<int>(reader, "AccountType"),
                SupplierSettings = Vanrise.Common.Serializer.Deserialize<Entities.CarrierAccountSupplierSettings>(reader["SupplierSettings"] as string),
                CustomerSettings = Vanrise.Common.Serializer.Deserialize<Entities.CarrierAccountCustomerSettings>(reader["CustomerSettings"] as string),
                SellingNumberPlanId = GetReaderValue<int?>(reader, "SellingNumberPlanID"),
                CarrierProfileId = (int)reader["CarrierProfileId"],
                CarrierAccountSettings = reader["CarrierAccountSettings"] as string != null ? Vanrise.Common.Serializer.Deserialize<Entities.CarrierAccountSettings>(reader["CarrierAccountSettings"] as string) : null,

            };
            return carrierAccount;
        }
        internal static DataTable BuildRoutingCustomerInfoTable(IEnumerable<RoutingCustomerInfo> customerInfos)
        {
            DataTable dtCustomerInfos = GetRoutingCustomerInfoTable();
            dtCustomerInfos.BeginLoadData();
            foreach (var c in customerInfos)
            {
                DataRow dr = dtCustomerInfos.NewRow();
                dr["CustomerId"] = c.CustomerId;
                dtCustomerInfos.Rows.Add(dr);
            }
            dtCustomerInfos.EndLoadData();
            return dtCustomerInfos;
        }
        private static DataTable GetRoutingCustomerInfoTable()
        {
            DataTable dtCustomerInfos = new DataTable();
            dtCustomerInfos.Columns.Add("CustomerId", typeof(Int32));
            return dtCustomerInfos;
        }
        internal static DataTable BuildRoutingSupplierInfoTable(IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            DataTable dtSupplierInfos = GetRoutingSupplierInfoTable();
            dtSupplierInfos.BeginLoadData();
            foreach (var s in supplierInfos)
            {
                DataRow dr = dtSupplierInfos.NewRow();
                dr["SupplierId"] = s.SupplierId;
                dtSupplierInfos.Rows.Add(dr);
            }
            dtSupplierInfos.EndLoadData();
            return dtSupplierInfos;
        }
        private static DataTable GetRoutingSupplierInfoTable()
        {
            DataTable dtSupplierInfos = new DataTable();
            dtSupplierInfos.Columns.Add("SupplierId", typeof(Int32));
            return dtSupplierInfos;
        }
        #endregion

    }
}
