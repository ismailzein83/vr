using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CarrierDataManager : BaseTOneDataManager, ICarrierDataManager
    {
        public List<CarrierAccountInfo> GetActiveSuppliersInfo()
        {
            return GetItemsSP("BEntity.sp_CarrierAccount_GetActiveSuppliersInfo", CarrierAccountInfoMapper);
        }

        public List<CarrierInfo> GetCarriers(CarrierType carrierType)
        {
            return GetItemsSP("BEntity.sp_Carriers_GetCarriers", (reader) =>
            {
                return new CarrierInfo
                {
                    CarrierAccountID = reader["CarrierAccountID"] as string,
                    Name = string.Format("{0}{1}", reader["Name"] as string, reader["NameSuffix"] != DBNull.Value && !string.IsNullOrEmpty(reader["NameSuffix"].ToString()) ? " (" + reader["NameSuffix"] as string + ")" : string.Empty)
                };
            }, carrierType.ToString());
        }
        public List<CarrierAccount> GetAllCarriers()
        {
            return GetItemsSP("BEntity.SP_Carriers_GetAllCarriers", (reader) =>
            {
                return new CarrierAccount
                {
                    CarrierAccountId = reader["CarrierAccountId"] as string,
                    ProfileId = (Int16)reader["ProfileId"],
                    ProfileName = reader["ProfileName"] as string,
                    ProfileCompanyName = reader["ProfileCompanyName"] as string,
                    ActivationStatus = (byte)reader["ActivationStatus"],
                    RoutingStatus = (byte)reader["RoutingStatus"],
                    AccountType = (byte)reader["AccountType"],
                    CustomerPaymentType = (byte)reader["CustomerPaymentType"],
                    SupplierPaymentType = (byte)reader["SupplierPaymentType"],
                    NameSuffix = reader["NameSuffix"] as string
                };
            });
        }

        public string GetCarrierAccountName(string carrierAccountId)
        {
            string carrierAccountName = GetItemSP("BEntity.sp_CarrierAccount_GetName",
                (reader) =>
                {
                    return string.Format("{0}{1}", reader["Name"] as string, reader["NameSuffix"] != DBNull.Value && !string.IsNullOrEmpty(reader["NameSuffix"] as string) ? " (" + reader["NameSuffix"] as string + ")" : string.Empty);
                },
                carrierAccountId);

            return carrierAccountName;
        }

        public List<CarrierAccount> GetCarrierAccounts(string name, string companyName, int from, int to)
        {
            return GetItemsSP("BEntity.sp_CarrierAccount_GetByName", (reader) =>
                {
                    return new CarrierAccount
                    {
                        CarrierAccountId = reader["CarrierAccountId"] as string,
                        ProfileId = (Int16)reader["ProfileId"],
                        ProfileName = reader["ProfileName"] as string,
                        ProfileCompanyName = reader["ProfileCompanyName"] as string,
                        ActivationStatus = (byte)reader["ActivationStatus"],
                        RoutingStatus = (byte)reader["RoutingStatus"],
                        AccountType = (byte)reader["AccountType"],
                        CustomerPaymentType = (byte)reader["CustomerPaymentType"],
                        SupplierPaymentType = (byte)reader["SupplierPaymentType"],
                        NameSuffix = reader["NameSuffix"] as string
                    };
                }, name, companyName, from, to);
        }
        public CarrierAccount GetCarrierAccount(string carrierAccountId)
        {
            return GetItemSP("BEntity.sp_CarrierAccount_GetByCarrierAccountId", (reader) =>
                {
                    return new CarrierAccount
                    {
                        CarrierAccountId = reader["CarrierAccountId"] as string,
                        ProfileId = (Int16)reader["ProfileId"],
                        ProfileName = reader["ProfileName"] as string,
                        ProfileCompanyName = reader["ProfileCompanyName"] as string,
                        ActivationStatus = (byte)reader["ActivationStatus"],
                        RoutingStatus = (byte)reader["RoutingStatus"],
                        AccountType = (byte)reader["AccountType"],
                        CustomerPaymentType = (byte)reader["CustomerPaymentType"],
                        SupplierPaymentType = (byte)reader["SupplierPaymentType"],
                        NameSuffix = reader["NameSuffix"] as string
                    };
                }, carrierAccountId);
        }
        public int InsertCarrierTest(string carrierAccountID, string Name)
        {

            int rowEffected = ExecuteNonQuerySP("BEntity.sp_InsertCarrierInfoTest", carrierAccountID, Name);
            return rowEffected;
        }
        public int UpdateCarrierAccount(CarrierAccount carrierAccount)
        {

            int rowEffected = ExecuteNonQuerySP("BEntity.sp_CarrierAccount_Update ",
                carrierAccount.AccountType,carrierAccount.ActivationStatus,carrierAccount.CarrierAccountId,
                carrierAccount.CustomerPaymentType,carrierAccount.NameSuffix,carrierAccount.ProfileCompanyName,
                carrierAccount.ProfileId,carrierAccount.ProfileName,carrierAccount.RoutingStatus,carrierAccount.SupplierPaymentType);
            return rowEffected;
        }

        #region Private Methods

        private CarrierAccountInfo CarrierAccountInfoMapper(IDataReader reader)
        {
            return new CarrierAccountInfo
            {
                CarrierAccountId = reader["CarrierAccountID"] as string
            };
        }

        internal static DataTable BuildCarrierAccountInfoTable(List<CarrierAccountInfo> carrierAccountsInfo)
        {
            DataTable dtSuppliersCodeInfo = new DataTable();
            dtSuppliersCodeInfo.Columns.Add("CarrierAccountID", typeof(string));
            dtSuppliersCodeInfo.BeginLoadData();
            foreach (var c in carrierAccountsInfo)
            {
                DataRow dr = dtSuppliersCodeInfo.NewRow();
                dr["CarrierAccountID"] = c.CarrierAccountId;
                dtSuppliersCodeInfo.Rows.Add(dr);
            }
            dtSuppliersCodeInfo.EndLoadData();
            return dtSuppliersCodeInfo;
        }

        #endregion
    }
}
