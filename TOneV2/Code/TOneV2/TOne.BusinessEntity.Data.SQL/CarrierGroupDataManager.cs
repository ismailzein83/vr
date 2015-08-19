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
    public class CarrierGroupDataManager : BaseTOneDataManager, ICarrierGroupDataManager 
    {

        public CarrierGroup GetCarrierGroup(int carrierGroupId)
        {
            return GetItemSP("BEntity.sp_CarrierGroup_GetById", CarrierGroupMapper, carrierGroupId);
        }

        public List<CarrierAccount> GetCarrierGroupMembers(IEnumerable<int> carrierGroupIds)
        {
            DataTable dtCarrierGroupMembers = BuildCarrierAccountInfoTable(carrierGroupIds);

            return GetItemsSPCmd("[BEntity].[sp_CarrierGroupMember_GetByCarrierGroupIds]",
                  CarrierAccountDataManager.CarrierAccountMapper,
                  (cmd) =>
                  {
                      var dtPrm = new SqlParameter("@CarrierGroupIds", SqlDbType.Structured);
                      dtPrm.Value = dtCarrierGroupMembers;
                      cmd.Parameters.Add(dtPrm);

                  });
        }

        public Vanrise.Entities.BigResult<CarrierAccount> GetCarrierGroupMembers(Vanrise.Entities.DataRetrievalInput<CarrierGroupQuery> input, IEnumerable<int> carrierGroupIds,List<string> filter)
        {
           // DataTable dtCarrierGroupMembers = BuildCarrierAccountInfoTable(carrierGroupIds);

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateQuery(tempTableName, filter, carrierGroupIds), (cmd) =>
                {
                });
            };
            return RetrieveData(input, createTempTableAction, CarrierAccountDataManager.CarrierAccountMapper);

            //return RetrieveData(input, (tempTableName) =>
            //{
            //    //tempTableName, lstCarrierGroupIds
            //    ExecuteNonQuerySPCmd("BEntity.sp_CarrierGroupMember_CreateTempForCarrierGroupIds", (cmd) =>
            //    {
            //        var dtPrm = new SqlParameter("@CarrierGroupIds", SqlDbType.Structured);
            //        dtPrm.Value = dtCarrierGroupMembers;
            //        cmd.Parameters.Add(dtPrm);

            //        var tempTableNamePrm = new SqlParameter("@TempTableName", SqlDbType.VarChar);
            //        tempTableNamePrm.Value = tempTableName;
            //        cmd.Parameters.Add(tempTableNamePrm);

            //    });

            //}, CarrierAccountDataManager.CarrierAccountMapper);
        }



        private string CreateQuery(string tempTableName, List<string> filter, IEnumerable<int> carrierGroupIds)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"

                                           
                                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                        BEGIN
			                                WITH CarrierAccountIDs (CarrierAccountID)
		                                	AS
			                                (
			                                	SELECT DISTINCT cgm.CarrierAccountID
			                                	FROM BEntity.CarrierGroupMember as cgm WHERE #CARRIERWHEREFILTER#
			                                ) , AllCarrierAccounts AS (
		    
				                            SELECT ca.CarrierAccountId,
					                                cp.ProfileId ,
						                            cp.Name AS ProfileName,
						                            cp.CompanyName AS ProfileCompanyName,
						                            ca.ActivationStatus,
						                            ca.RoutingStatus,
						                            ca.AccountType,
						                            ca.CustomerPaymentType,
						                            ca.SupplierPaymentType,
						                            ca.NameSuffix,
						                            '' as CarrierAccountName
			                                        FROM CarrierAccount  as ca WITH(NOLOCK) Join CarrierAccountIDs as caIds on ca.CarrierAccountID = caIds.CarrierAccountID
										            INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
                                                    #WHERESTRING#
                                                     #FILTER#
			                                )
			
			                                SELECT * INTO #TEMPTABLE#  FROM AllCarrierAccounts
		                                    END
                                                ");
            StringBuilder whereString = new StringBuilder();
            StringBuilder carrierAccountWhereBuilder = new StringBuilder();
            HashSet<string> joinStatement = new HashSet<string>();
            AddFilter<string>(whereBuilder, filter, "ca.CarrierAccountId");

            AddFilter<int>(carrierAccountWhereBuilder, carrierGroupIds, "cgm.CarrierGroupID");

            if (whereBuilder.Length > 0)
                whereString.Append(" Where ");
            queryBuilder.Replace("#CARRIERWHEREFILTER#", carrierAccountWhereBuilder.ToString());
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#WHERESTRING#", whereString.ToString());
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            return queryBuilder.ToString();
        }

        public void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (whereBuilder.Length!=0)
                whereBuilder.AppendFormat(" AND " );
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat(" {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat(" {0} IN ({1})", column, String.Join(", ", values));
            }
        }



        internal static DataTable BuildCarrierAccountInfoTable(IEnumerable<int> carrierGroupIds)
        {
            DataTable dtCarrierAccountInfo = new DataTable();
            dtCarrierAccountInfo.Columns.Add("ID", typeof(int));
            dtCarrierAccountInfo.BeginLoadData();
            foreach (var z in carrierGroupIds)
            {
                DataRow dr = dtCarrierAccountInfo.NewRow();
                dr["ID"] = z;
                dtCarrierAccountInfo.Rows.Add(dr);
            }
            dtCarrierAccountInfo.EndLoadData();
            return dtCarrierAccountInfo;
        }

        public bool AddCarrierGroup(Entities.CarrierGroup carrierGroup, string[] CarrierAccountIds, out int insertedId)
        {
            object carrierGroupId;

            int recordesEffected = ExecuteNonQuerySP("BEntity.sp_CarrierGroup_Insert", out carrierGroupId, carrierGroup.Name, carrierGroup.ParentID);

            foreach (string carrAccountId in CarrierAccountIds)
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierGroupMember_Insert", carrierGroupId, carrAccountId);
            }
            insertedId = (recordesEffected > 0) ? (Int16)carrierGroupId : -1;
            return (recordesEffected > 0);
        }

        public bool UpdateCarrierGroup(Entities.CarrierGroup carrierGroup, string[] CarrierAccountIds)
        {
            //update the Carrier group Information and delete the old carrier groups members
            int recordesEffected = ExecuteNonQuerySP("BEntity.sp_CarrierGroup_Update", carrierGroup.ID, carrierGroup.Name, carrierGroup.ParentID);

            //Insert the new carrier groups members
            foreach (string carrAccountId in CarrierAccountIds)
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierGroupMember_Insert", carrierGroup.ID, carrAccountId);
            }

            if (recordesEffected > 0)
                return true;
            return false;
        }

        Entities.CarrierGroup CarrierGroupMapper(IDataReader reader)
        {
            Entities.CarrierGroup module = new Entities.CarrierGroup
            {
                ID = (int)reader["ID"],
                Name = reader["Name"] as string,
                ParentID = GetReaderValue<int?>(reader, "ParentID")
            };
            return module;
        }



    }
}
