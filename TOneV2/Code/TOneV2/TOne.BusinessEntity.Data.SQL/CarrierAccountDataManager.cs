using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CarrierAccountDataManager : BaseTOneDataManager, ICarrierAccountDataManager
    {
        public List<CarrierAccountInfo> GetActiveSuppliersInfo()
        {
            return GetItemsSP("BEntity.sp_CarrierAccount_GetActiveSuppliersInfo", CarrierAccountInfoMapper);
        }

        public List<CarrierInfo> GetCarriers(CarrierType carrierType, List<string> assignedCarriers)
        {
            List<CarrierInfo> carriers = new List<CarrierInfo>();
            ExecuteReaderText(CreateQuery(assignedCarriers, carrierType), (reader) =>
            { 
               while (reader.Read())
               {           
                  try
                  {
                   CarrierInfo carrierInfo = new CarrierInfo
                    {
                      CarrierAccountID = reader["CarrierAccountID"] as string,
                      Name = GetCarrierAccountName(reader["Name"] as string, reader["NameSuffix"] as string)
                    };
                    carriers.Add(carrierInfo);
                  }
                 catch (Exception ex)
                 {
                    throw ex;
                 }
               }
             }, (cmd) =>
             {
               cmd.Parameters.Add(new SqlParameter("@CarrierType", carrierType.ToString()));
             });
            return carriers;
        }

        public List<CarrierAccount> GetAllCarriers(CarrierType carrierType)
        {
            return GetItemsSP("BEntity.sp_CarrierAccount_GetAll", CarrierAccountMapper, (int)carrierType);
        }


        private string CreateQuery(List<string> assignedCarriers, CarrierType carrierType)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder accountType= new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"                  
                                                    SELECT ca.CarrierAccountID, cp.Name, ca.NameSuffix  FROM CarrierAccount ca
                                                    INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
                                                    WHERE ca.IsDeleted = 'N' AND ca.ActivationStatus = 2 #AddAccountTypeValues# #FILTER#
                                                    ORDER BY Name ASC
                                                ");

            AddFilter<string>(whereBuilder, assignedCarriers, "ca.CarrierAccountID");
            AddAccountTypeValues(carrierType, accountType);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            queryBuilder.Replace("#AddAccountTypeValues#", accountType.ToString());
            return queryBuilder.ToString();
        }
        private void AddAccountTypeValues(CarrierType carrierType,StringBuilder accountType)
        {
            
            if(carrierType==CarrierType.Customer)
                accountType.Append("AND ca.AccountType IN (0,1)");
            else if(carrierType==CarrierType.Supplier)
                accountType.Append("AND ca.AccountType IN (2,1)");
            else
                accountType.Append(" ");

        }

        public void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (whereBuilder.Length != 0)
                whereBuilder.AppendFormat(" AND ");
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
            }
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
                        NominalCapacityInE1s=GetReaderValue<int>(reader, "NominalCapacityInE1s"),
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
                carrierAccount.ProfileId,carrierAccount.ProfileName,carrierAccount.RoutingStatus,carrierAccount.SupplierPaymentType, carrierAccount.CarrierMaskId);
            return rowEffected;
        }

        public int UpdateCarrierAccountGroup(CarrierAccount carrierAccount)
        {
            int rowEffected = ExecuteNonQuerySP("BEntity.sp_CarrierAccount_UpdateGroups",
                carrierAccount.CarrierAccountId, carrierAccount.CarrierGroupID, carrierAccount.CarrierGroups);
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

        internal static CarrierAccount CarrierAccountMapper(IDataReader reader)
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
                CarrierAccountName = GetCarrierAccountName(reader["ProfileName"] as string, reader["NameSuffix"] as string)
            };
        }
        
        internal static string GetCarrierAccountName(string carrierName, string nameSuffix)
        {
            return string.Format("{0}{1}", carrierName, string.IsNullOrEmpty(nameSuffix) ? string.Empty : " (" + nameSuffix + ")");
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
                if (CarrierGroupID != "")
                {
                    carrierGroupId = int.Parse(CarrierGroupID);
                    lstcarrierGroups.Add(carrierGroupId);
                }
            }
            return lstcarrierGroups;
        }

        Dictionary<string, CarrierAccount> ICarrierAccountDataManager.GetAllCarrierAccounts()
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

        Dictionary<int, CarrierGroup> ICarrierAccountDataManager.GetAllCarrierGroups()
        {
            Dictionary<int, CarrierGroup> dic = new Dictionary<int, CarrierGroup>();

            ExecuteReaderSP("BEntity.sp_CarrierGroup_GetAll", (reader) =>
            {
                while (reader.Read())
                    dic.Add((int)reader["ID"], new CarrierGroup
                    {
                        ID = (int)reader["ID"],
                        Name = reader["Name"] as string,
                        ParentID =GetReaderValue<int?>(reader,"ParentID"),
                    });
                
            });
            return dic;
        }
    }
}
