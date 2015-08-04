using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.Analytics.Entities;
using TOne.Analytics.Data;
using System.Data;
using System.Data.SqlClient;
using TOne.Entities;
using TOne.BusinessEntity.Business;
using Vanrise.Entities;
namespace TOne.Analytics.Data.SQL
{
    class CDRDataManager : BaseTOneDataManager, ICDRDataManager
    {
        public Vanrise.Entities.BigResult<BillingCDR> GetCDRData(Vanrise.Entities.DataRetrievalInput<CDRSummaryInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SwitchName", "SwitchID");
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter, input.Query.CDROption), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
                    cmd.Parameters.Add(new SqlParameter("@nRecords", input.Query.Size));
                });
            };

            Vanrise.Entities.BigResult<BillingCDR> cdrData=RetrieveData(input, createTempTableAction, CDRDataMapper, mapper);
             FillCDRList(cdrData);
             return cdrData;
        }

        //public CDRBigResult GetCDRData(string tempTableKey, int nRecords)
        //{
        //    TempTableName tempTableName = null;
        //    if (tempTableKey != null)
        //        tempTableName = GetTempTableName(tempTableKey);
        //    CDRBigResult rslt = new CDRBigResult()
        //    {
        //        ResultKey = tempTableName.Key
        //    };

        //    int totalDataCount;
        //    rslt.Data = GetData(tempTableName.TableName, 0, nRecords, BillingCDRMeasures.Attempt, true, out totalDataCount);
        //    rslt.TotalCount = totalDataCount;

        //    return rslt;
        //}

        #region Methods
        private string CreateTempTableIfNotExists(string tempTableName, CDRFilter filter, BillingCDROptionMeasures CDROption)
        {
            string queryData = "";
            switch(CDROption)
            {
                case BillingCDROptionMeasures.All: queryData = String.Format(@"{0} UNION ALL {1}", GetSingleQuery("Billing_CDR_Main", "MainCDR"), GetSingleQuery("Billing_CDR_Invalid", "Invalid"));
                    break;
                case BillingCDROptionMeasures.Invalid: queryData = GetSingleQuery("Billing_CDR_Invalid", "Invalid"); break;
                case BillingCDROptionMeasures.Successful: queryData = GetSingleQuery("Billing_CDR_Main", "MainCDR"); break;
            }
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN

			                        SELECT Top (@nRecords) newtable.* INTO #TEMPTABLE# FROM (#Query#)as newtable  where (Attempt between @FromDate AND @ToDate) #FILTER#


                            END
                                ");
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#Query#", queryData);
            AddFilterToQuery(filter, whereBuilder);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            return queryBuilder.ToString();
       
        }

        private void FillCDRList(Vanrise.Entities.BigResult<BillingCDR> cdrData)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();
            foreach(BillingCDR data in cdrData.Data)
            {
                if (data.OurZoneID!=0)
                data.OurZoneName = manager.GetZoneName(data.OurZoneID);
                if (data.CustomerID != null)
                data.CustomerInfo = manager.GetCarrirAccountName(data.CustomerID);
                if (data.OriginatingZoneID != 0)
                data.OriginatingZoneName = manager.GetZoneName(data.OriginatingZoneID);
                if (data.SupplierID != null)
                data.SupplierName = manager.GetCarrirAccountName(data.SupplierID);
                if (data.SupplierZoneID != 0)
                data.SupplierZoneName = manager.GetZoneName(data.SupplierZoneID);
                if (data.SwitchID != 0)
                data.SwitchName = manager.GetSwitchName(data.SwitchID);
            }
        }

        private string GetColumnName(BillingCDRMeasures column)
        {
            switch (column)
            {
                case BillingCDRMeasures.Attempt: return "Attempt";
                case BillingCDRMeasures.CDPN: return "CDPN";
                case BillingCDRMeasures.CDPNOut: return "CDPNOut";
                case BillingCDRMeasures.CGPN: return "CGPN";
                case BillingCDRMeasures.CustomerInfo: return "CustomerInfo";
                case BillingCDRMeasures.DurationInSeconds: return "DurationInSeconds";
                case BillingCDRMeasures.OriginatingZoneName: return "OriginatingZoneName";
                case BillingCDRMeasures.OurZoneName: return "OurZoneName";
                case BillingCDRMeasures.PDD: return "PDD";
                case BillingCDRMeasures.ReleaseCode: return "ReleaseCode";
                case BillingCDRMeasures.ReleaseSource : return "ReleaseSource";
                case BillingCDRMeasures.SupplierName: return "SupplierName";
                case BillingCDRMeasures.SupplierZoneName: return "SupplierZoneName";
                case BillingCDRMeasures.SwitchName: return "SwitchName";
                default: return column.ToString();
            }
        }
        private string GetSingleQuery(string tableName,string alias){
            StringBuilder whereBuilder = new StringBuilder();
            return String.Format(@"
                        SELECT 
                        {1}.ID,
                        DATEADD(ms,-datepart(ms,Attempt),Attempt) AS Attempt,
                        {1}.Alert,
                        {1}.Connect,
                        DATEDIFF(s,{1}.Attempt, (CASE WHEN {1}.Alert IS NULL THEN ( CASE WHEN {1}.Connect IS NULL THEN (NULL) ELSE ({1}.Connect) END ) ELSE ({1}.Alert) END )) AS PDD,
                        {1}.CDPN,
                        {1}.CDPNOut ,
                        {1}.CGPN,
                        {1}.ReleaseCode,
                        {1}.ReleaseSource,
                        CONVERT(DECIMAL(10,2), {1}.DurationInSeconds) as DurationInSeconds,
                        {1}.SupplierZoneID,
                        {1}.SupplierID,
                        {1}.OriginatingZoneID ,
                        {1}.OurZoneID,
                        {1}.CustomerID,   
                        {1}.SwitchID ,
                        {1}.SwitchCdrID,
                        {1}.Tag,
                        {1}.Extra_Fields FROM {0} {1} 
                            ", tableName, alias);
                        }
        
        private BillingCDR CDRDataMapper(IDataReader reader)
        {

            BillingCDR instance = new BillingCDR();       
            FillCDRDataFromReader(instance, reader);
            return instance;
        }
        private void AddFilterToQuery(CDRFilter filter, StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "newtable.SwitchId");
            AddFilter(whereBuilder, filter.CustomerIds, "newtable.CustomerID");
            AddFilter(whereBuilder, filter.SupplierIds, "newtable.SupplierID");
            AddFilter(whereBuilder, filter.ZoneIds, "newtable.OurZoneID");
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

        void FillCDRDataFromReader(BillingCDR cdr, IDataReader reader)
        {
                
                cdr.Attempt = GetReaderValue<DateTime>(reader,"Attempt");
                cdr.PDD = GetReaderValue<int>(reader, "PDD");
                cdr.Alert = GetReaderValue<DateTime>(reader, "Alert");
                cdr.Connect = GetReaderValue<DateTime>(reader, "Connect");
                cdr.DurationInSeconds = GetReaderOfNumeric(reader, "DurationInSeconds");
                cdr.CustomerID = reader["CustomerID"] as string;
                cdr.OurZoneID = GetReaderValue<int>(reader, "OurZoneID");
                cdr.OriginatingZoneID = GetReaderValue<int>(reader, "OriginatingZoneID");
                cdr.SupplierID = reader["SupplierID"] as string;
                cdr.SupplierZoneID = GetReaderValue<int>(reader, "SupplierZoneID");
                cdr.CDPN = reader["CDPN"] as string;
                cdr.CGPN = reader["CGPN"] as string;
                cdr.ReleaseCode = reader["ReleaseCode"] as string;
                cdr.ReleaseSource = reader["ReleaseSource"] as string;
                cdr.SwitchID = GetReaderValueOfTinyInt(reader,"SwitchID");
                cdr.SwitchCdrID = GetReaderValue<Int64>(reader, "SwitchCdrID");
                cdr.Tag = reader["Tag"] as string;
                cdr.Extra_Fields = reader["Extra_Fields"] as string;
                cdr.CDPNOut = reader["CDPNOut"] as string;

        }
        public int GetReaderOfNumeric(IDataReader reader, String value)
        {
            return reader[value] != DBNull.Value ? Convert.ToInt32(reader[value]) : 0;
        }
        public int GetReaderValueOfTinyInt(IDataReader reader, String value)
        {
            return reader[value] != DBNull.Value ? Convert.ToInt32(reader[value]) : 0;
        }
        #endregion
    }
}
