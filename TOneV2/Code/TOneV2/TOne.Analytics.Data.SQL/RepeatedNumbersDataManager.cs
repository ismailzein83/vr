using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class RepeatedNumbersDataManager : BaseTOneDataManager, IRepeatedNumbersDataManager
    {
        bool isSwitch = false;
        public Vanrise.Entities.BigResult<RepeatedNumbers> GetRepeatedNumbersData(Vanrise.Entities.DataRetrievalInput<RepeatedNumbersInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("CustomerInfo", "CustomerID");
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.SwitchIds), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
                    cmd.Parameters.Add(new SqlParameter("@Type", input.Query.Type));
                    cmd.Parameters.Add(new SqlParameter("@PhoneNumberType", input.Query.PhoneNumberType));
                    cmd.Parameters.Add(new SqlParameter("@SwitchID", ""));
                    cmd.Parameters.Add(new SqlParameter("@Number", input.Query.Number));
                });
            };

           Vanrise.Entities.BigResult < RepeatedNumbers > repeatedNumbersData=  RetrieveData(input, createTempTableAction, BlockedAttemptsMapper, mapper);
             FillRepeatedNumbersList( repeatedNumbersData);
             return repeatedNumbersData;
        }
        RepeatedNumbers BlockedAttemptsMapper(IDataReader reader)
        {
            RepeatedNumbers obj = new RepeatedNumbers();
            BlockedAttemptsFromReader(obj, reader);
            return obj;
        }
        void BlockedAttemptsFromReader(RepeatedNumbers repeatedNumbers, IDataReader reader)
        {
            repeatedNumbers.OurZoneID = GetReaderValue<int>(reader, "OurZoneID");
            repeatedNumbers.CustomerID = reader["CustomerID"] as string !=null?reader["CustomerID"] as string:"N/A";
            repeatedNumbers.SupplierID = reader["SupplierID"] as string != null ? reader["SupplierID"] as string : "N/A";
            repeatedNumbers.PhoneNumber = reader["PhoneNumber"] as string != null ? reader["PhoneNumber"] as string : "N/A";
            repeatedNumbers.Attempts = GetReaderValue<int>(reader, "Attempts");
            if(isSwitch)
            repeatedNumbers.SwitchID = GetReaderValue<Byte>(reader, "SwitchID");
            repeatedNumbers.DurationsInMinutes =GetReaderValue<Decimal>(reader, "DurationsInMinutes");

        }
        public string CreateTempTableIfNotExists(string tempTableName, List<int> switchIds)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder whereBuilder1 = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN 
                                SELECT #SELECTPART#  N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID, Sum(N.Attempt) Attempts, Sum(N.DurationsInMinutes) DurationsInMinutes
                                    INTO #TEMPTABLE#
                                    FROM
                                ( 
	                                SELECT  

                                    #SELECTPART#
		                            BM.OurZoneID AS OurZoneID,
		                            BM.CustomerID,
		                            BM.SupplierID,
		                            Count(BM.Attempt) as Attempt, 
		                            Sum (BM.DurationInSeconds)/60. as DurationsInMinutes ,
		                            CASE @PhoneNumberType
			                        WHEN 'CDPN' THEN BM.CDPN 
			                        WHEN 'CGPN' THEN BM.CGPN 
		                            END
		                            AS PhoneNumber
	                                FROM dbo.Billing_CDR_Main BM WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt)) 
	                                WHERE 
			                        Attempt BETWEEN @FromDate AND @ToDate
		                            AND @Type IN ('ALL', 'SUCCESSFUL')
		                             #FILTER#
	                                GROUP BY BM.OurZoneID, BM.CustomerID, BM.SupplierID #GROUPBYPART#,
	                                CASE @PhoneNumberType
		                            WHEN 'CDPN' THEN BM.CDPN 
		                            WHEN 'CGPN' THEN BM.CGPN 
	                                END
	                                UNION ALL
	                                SELECT  
                                    #SELECTPART#
		                            BI.OurZoneID AS OurZoneID,
		                            BI.CustomerID,
		                            BI.SupplierID,
		                            Count(BI.Attempt) as Attempt, 
		                            Sum (BI.DurationInSeconds) / 60. as DurationsInMinutes,
		                            CASE @PhoneNumberType
			                        WHEN 'CDPN' THEN BI.CDPN 
			                        WHEN 'CGPN' THEN BI.CGPN 
		                            END
		                            AS PhoneNumber
	                                FROM dbo.Billing_CDR_Invalid BI WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt)) 
	                                WHERE 
			                        Attempt BETWEEN @FromDate AND @ToDate
                                   
		                            AND @Type IN ('ALL', 'FAILED')
                                    #FILTER1# 
	                                GROUP BY BI.OurZoneID,  BI.CustomerID, BI.SupplierID #GROUPBYPART#,
	                                CASE @PhoneNumberType
		                             WHEN 'CDPN' THEN BI.CDPN 
		                            WHEN 'CGPN' THEN BI.CGPN 
	                                END
	
                                    ) N
                                    GROUP BY N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID #GROUPBYPART# 
                                    HAVING Sum(N.Attempt) >= @Number 
                                    ORDER BY Sum(N.Attempt) DESC 
                             
                            END");
            AddFilter(whereBuilder, switchIds, "BM.SwitchId");
            AddSecondFilter(whereBuilder1, switchIds, "BI.SwitchId");
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            queryBuilder.Replace("#FILTER1#", whereBuilder1.ToString());
            if (switchIds.Count > 0){
                isSwitch = true;
                queryBuilder.Replace("#SELECTPART#", "SwitchID,");
                queryBuilder.Replace("#GROUPBYPART#", ",SwitchID");
            }

            else
            {
                isSwitch = false;
                queryBuilder.Replace("#SELECTPART#", "");
                queryBuilder.Replace("#GROUPBYPART#", "");
            }
               

            return queryBuilder.ToString();
        }

        void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
            }

        }
        void AddSecondFilter(StringBuilder whereBuilder, List<int> switchIds, string column)
        {
            if (switchIds.Count > 0)
            {
                whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", switchIds));
            }
            else
                whereBuilder.AppendFormat("AND (CustomerID IS NOT NULL AND BI.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) ");
        }
        private void FillRepeatedNumbersList(Vanrise.Entities.BigResult<RepeatedNumbers> repeatedNumbersData)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();
            foreach (RepeatedNumbers data in repeatedNumbersData.Data)
            {
                if (data.OurZoneID != 0)
                    data.OurZoneName = manager.GetZoneName(data.OurZoneID);
                else
                    data.OurZoneName = "N/A";
                if (data.CustomerID != null || data.CustomerID.Length != 0)
                    data.CustomerInfo = manager.GetCarrirAccountName(data.CustomerID);
                else
                    data.CustomerInfo = "N/A";
                if (data.SupplierID != null || data.SupplierID.Length!=0)
                    data.SupplierName = manager.GetCarrirAccountName(data.SupplierID);
                else
                    data.SupplierName = "N/A";
   
                data.SwitchName = manager.GetSwitchName(data.SwitchID)!=null?manager.GetSwitchName(data.SwitchID):"N/A";
            }
        }
    }
}
