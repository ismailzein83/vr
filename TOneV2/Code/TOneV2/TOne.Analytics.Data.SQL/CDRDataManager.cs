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
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static CDRDataManager()
        {
            _columnMapper.Add("SwitchName", "SwitchID");
        }

        public Vanrise.Entities.BigResult<BillingCDR> GetCDRData(Vanrise.Entities.DataRetrievalInput<CDRSummaryInput> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter, input.Query.CDROption), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
                    cmd.Parameters.Add(new SqlParameter("@nRecords", input.Query.Size));
                });
            };

            Vanrise.Entities.BigResult<BillingCDR> cdrData=RetrieveData(input, createTempTableAction, CDRDataMapper, _columnMapper);
             FillCDRList(cdrData);
             return cdrData;
        }


        #region Methods
        private string CreateTempTableIfNotExists(string tempTableName, CDRFilter filter, BillingCDROptionMeasures CDROption)
        {
            string queryData = "";
            StringBuilder whereBuilder = new StringBuilder();
            AddFilterToQuery(filter, whereBuilder);
            switch(CDROption)
            {
                case BillingCDROptionMeasures.All: queryData = String.Format(@"{0} UNION ALL {1}", GetSingleQuery("Billing_CDR_Main", "MainCDR", tempTableName, whereBuilder, "'N' as IsRerouted"), GetSingleQuery("Billing_CDR_Invalid", "Invalid", null, whereBuilder, "Invalid.IsRerouted"));
                    break;
                case BillingCDROptionMeasures.Invalid: queryData = GetSingleQuery("Billing_CDR_Invalid", "Invalid", tempTableName, whereBuilder, "Invalid.IsRerouted"); break;
                case BillingCDROptionMeasures.Successful: queryData = GetSingleQuery("Billing_CDR_Main", "MainCDR", tempTableName, whereBuilder, "'N' as IsRerouted"); break;
            }
          
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN 
                        Select TOP(@nRecords) newtable.* INTO #TEMPTABLE# FROM (#Query#) as newtable

                                 END
                                ");
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#Query#", queryData);
           

           
        
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


        private string GetSingleQuery(string tableName, string alias, string tempTableName,StringBuilder whereBuilder,string isRerouted)
        {
            return String.Format(@"
                                    
                        SELECT TOP(@nRecords)
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
                        {1}.Extra_Fields,
                        {3} FROM {0} {1} 
                            where ({1}.Attempt between @FromDate AND @ToDate)  {2}  ", tableName, alias, whereBuilder.ToString(), isRerouted);
                        }
        
        private BillingCDR CDRDataMapper(IDataReader reader)
        {

            BillingCDR instance = new BillingCDR();       
            FillCDRDataFromReader(instance, reader);
            return instance;
        }
        private void AddFilterToQuery(CDRFilter filter, StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "SwitchId");
            AddFilter(whereBuilder, filter.CustomerIds, "CustomerID");
            AddFilter(whereBuilder, filter.SupplierIds, "SupplierID");
            AddFilter(whereBuilder, filter.ZoneIds, "OurZoneID");
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
                cdr.DurationInSeconds = GetReaderValue<Decimal>(reader, "DurationInSeconds");
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
                cdr.IsRerouted = reader["IsRerouted"] as string;

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
