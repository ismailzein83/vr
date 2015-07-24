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

        public Vanrise.Entities.BigResult<CarrierAccount> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
                {
                    ExecuteNonQuerySP("[BEntity].[SP_CarrierAccount_CreateTempForFiltered]", tempTableName, input.Query.Name, input.Query.CompanyName);
                };
            return RetrieveData(input, createTempTableAction, CarrierAccountMapper);
        }

        CarrierAccount CarrierAccountMapper(IDataReader reader)
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
                        NameSuffix = reader["NameSuffix"] as string,
                        CarrierGroupName = reader["CarrierGroupName"] as string,
                        CarrierGroups = reader["CarrierGroups"] as string
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

        private List<int> SplitGroups(string CarrierGroups)
        {
            char[] seperator = { ',' };
            List<int> lstcarrierGroups = new List<int>();
            int carrierGroupId;

            if (CarrierGroups == null || CarrierGroups == "") return null;
            if (CarrierGroups.ToString().Split(seperator).Length == 0) return null;

            foreach (string CarrierGroupID in CarrierGroups.ToString().Split(seperator))
            {
                carrierGroupId = int.Parse(CarrierGroupID);
                lstcarrierGroups.Add(carrierGroupId);
            }
            return lstcarrierGroups;
        }

        Dictionary<string, CarrierAccount> ICarrierDataManager.GetAllCarrierAccounts()
        {
            Dictionary<string, CarrierAccount> dic = new Dictionary<string, CarrierAccount>();

            ExecuteReaderSP("BEntity.SP_Carriers_GetAllCarriers", (reader) =>
            {
                while (reader.Read())
                    dic.Add(reader["CarrierAccountId"] as string, new CarrierAccount
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
                        NameSuffix = reader["NameSuffix"] as string,
                        GroupIds = SplitGroups(reader["CarrierGroups"] as string),
                        CarrierGroupID = GetReaderValue<int?>(reader,"CarrierGroupID")
                    });
            });
            return dic;
        }

        Dictionary<int, CarrierGroup> ICarrierDataManager.GetAllCarrierGroups()
        {
            Dictionary<int, CarrierGroup> dic = new Dictionary<int, CarrierGroup>();

            ExecuteReaderSP("BEntity.sp_CarrierGroup_GetAllCarrierGroup", (reader) =>
            {
                while (reader.Read())
                    dic.Add((int)reader["CarrierGroupID"], new CarrierGroup
                    {
                        CarrierGroupID = (int)reader["CarrierGroupID"],
                        CarrierGroupName = reader["CarrierGroupName"] as string,
                        ParentID =GetReaderValue<int?>(reader,"ParentID"),
                        ParentPath = reader["ParentPath"] as string,
                        Path = reader["Path"] as string
                    });
                
            });
            return dic;
        }
    }
}
