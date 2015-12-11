using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using System.Data;
using System.Data.SqlClient;
using Vanrise.Entities;
using TOne.WhS.Analytics.Entities;
namespace TOne.WhS.Analytics.Data.SQL
{
    public class CDRDataManager : BaseTOneDataManager, ICDRDataManager
    {

        #region ctor/Local Variables
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        private string mainCDRTableName="[TOneWhS_CDR].[CDRMain]";
        private string mainCDRTableAlias = "MainCDR";
        private string mainCDRTableIndex = "IX_CDRMain_Attempt";

        private string invalidCDRTableName = "[TOneWhS_CDR].[CDRInvalid]";
        private string invalidCDRTableAlias = "InValidCDR";
        private string invalidCDRTableIndex = "IX_CDRInvalid_Attempt";

        private string failedCDRTableName = "[TOneWhS_CDR].[CDRFailed]";
        private string failedCDRTableAlias = "FailedCDR";
        private string failedCDRTableIndex = "IX_CDRFailed_Attempt";

        static CDRDataManager()
        {
            _columnMapper.Add("Entity.ID", "ID");
            _columnMapper.Add("SwitchName", "SwitchID");
            _columnMapper.Add("SaleZoneName", "SaleZoneID");
            _columnMapper.Add("CustomerName", "CustomerID");
            _columnMapper.Add("SupplierZoneName", "SupplierZoneID");
            _columnMapper.Add("SupplierName", "SupplierID");
            _columnMapper.Add("Entity.DurationInSeconds", "DurationInSeconds");
            _columnMapper.Add("Entity.Connect", "Connect");
            _columnMapper.Add("Entity.Alert", "Alert");
            _columnMapper.Add("Entity.PDD", "PDD");
            _columnMapper.Add("Entity.Attempt", "Attempt");
            _columnMapper.Add("Entity.CDPN", "CDPN");
            _columnMapper.Add("Entity.ReleaseCode", "ReleaseCode");
            _columnMapper.Add("Entity.CGPN", "CGPN");
            _columnMapper.Add("Entity.ReleaseSource", "ReleaseSource");
        }
        public CDRDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public Vanrise.Entities.BigResult<CDRLog> GetCDRLogData(Vanrise.Entities.DataRetrievalInput<CDRLogInput> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter, input.Query.CDRType), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(input.Query.To)));
                    cmd.Parameters.Add(new SqlParameter("@nRecords", input.Query.NRecords));
                });
            };
            return RetrieveData(input, createTempTableAction, CDRDataMapper, _columnMapper);
        }

        #endregion

        #region Private Methods
        private string CreateTempTableIfNotExists(string tempTableName, CDRLogFilter filter, CDRType cdrType)
        {
            string queryData = "";
            switch (cdrType)
            {
                case CDRType.All:
                    queryData = String.Format(@"{0} UNION ALL {1}  UNION ALL {2}", GetSingleQuery(mainCDRTableName, mainCDRTableAlias, filter, mainCDRTableIndex), GetSingleQuery(invalidCDRTableName, invalidCDRTableAlias, filter, invalidCDRTableIndex), GetSingleQuery(failedCDRTableName, failedCDRTableAlias, filter, failedCDRTableIndex)); 
                    break;
                case CDRType.Invalid:
                    queryData = GetSingleQuery(invalidCDRTableName, invalidCDRTableAlias, filter, invalidCDRTableIndex); 
                    break;
                case CDRType.Failed:
                    queryData = GetSingleQuery(failedCDRTableName, failedCDRTableAlias, filter, failedCDRTableIndex); 
                    break;
                case CDRType.Successful:
                    queryData = GetSingleQuery(mainCDRTableName, mainCDRTableAlias, filter, mainCDRTableIndex); 
                    break;
            }

            StringBuilder queryBuilder = new StringBuilder(@" IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
                                                              BEGIN 
                                                                Select TOP(@nRecords) newtable.* INTO #TEMPTABLE# FROM (#Query#) as newtable
                                                              END");
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#Query#", queryData);
            return queryBuilder.ToString();
        }
        private string GetSingleQuery(string tableName, string alias, CDRLogFilter filter,string tableIndex)
        {
            StringBuilder queryBuilder = new StringBuilder();
            StringBuilder selectQueryPart = new StringBuilder();
            StringBuilder whereBuilder = new StringBuilder();
            AddFilterToQuery(filter, whereBuilder);
            queryBuilder.Append(String.Format(@"SELECT TOP(@nRecords)    
                                               #SELECTPART#
                                               FROM {0} {1} WITH(NOLOCK ,INDEX(#TABLEINDEX#))
                                               WHERE ({1}.Attempt>= @FromDate AND ({1}.Attempt<= @ToDate OR @ToDate IS NULL))  #WHEREPART# ", tableName, alias));
           
            selectQueryPart.Append(String.Format(@"
                        
                        {0}.ID,
                        DATEADD(ms,-datepart(ms,{0}.Attempt),{0}.Attempt) AS Attempt,
                        {0}.Alert,
                        {0}.Connect,
                        DATEDIFF(s,{0}.Attempt, (CASE WHEN {0}.Alert IS NULL THEN ( CASE WHEN {0}.Connect IS NULL THEN (NULL) ELSE ({0}.Connect) END ) ELSE ({0}.Alert) END )) AS PDD,
                        {0}.CDPN,
                        {0}.CGPN,
                        {0}.ReleaseCode,
                        {0}.ReleaseSource,
                        {0}.DurationInSeconds,
                        {0}.SupplierZoneID,
                        {0}.SupplierID,
                        {0}.SaleZoneID,
                        {0}.CustomerID,   
                        {0}.SwitchID ", alias));

            queryBuilder.Replace("#SELECTPART#", selectQueryPart.ToString());
            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());
            queryBuilder.Replace("#TABLEINDEX#", tableIndex);

            return queryBuilder.ToString();

        }
        private CDRLog CDRDataMapper(IDataReader reader)
        {

            CDRLog cdr = new CDRLog();
            cdr.ID = GetReaderValue<long>(reader, "ID");
            cdr.Attempt = GetReaderValue<DateTime>(reader, "Attempt");
            cdr.PDD = GetReaderValue<int>(reader, "PDD");
            cdr.Alert = GetReaderValue<DateTime>(reader, "Alert");
            cdr.Connect = GetReaderValue<DateTime>(reader, "Connect");
            cdr.DurationInSeconds = GetReaderValue<int>(reader, "DurationInSeconds");
            cdr.CustomerID = GetReaderValue<int>(reader, "CustomerID");
            cdr.SaleZoneID = GetReaderValue<long>(reader, "SaleZoneID");
          //  cdr.OriginatingZoneID = GetReaderValue<int>(reader, "OriginatingZoneID");
            cdr.SupplierID = GetReaderValue<int>(reader, "SupplierID");
            cdr.SupplierZoneID = GetReaderValue<long>(reader, "SupplierZoneID");
            cdr.CDPN = reader["CDPN"] as string;
            cdr.CGPN = reader["CGPN"] as string;
            cdr.ReleaseCode = reader["ReleaseCode"] as string;
            cdr.ReleaseSource = reader["ReleaseSource"] as string;
            cdr.SwitchID = GetReaderValue<int>(reader, "SwitchID");
       //     cdr.IsRerouted = reader["IsRerouted"] as string;
            return cdr;
        }
        private void AddFilterToQuery(CDRLogFilter filter, StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "SwitchID");
            AddFilter(whereBuilder, filter.CustomerIds, "CustomerID");
            AddFilter(whereBuilder, filter.SupplierIds, "SupplierID");
            AddFilter(whereBuilder, filter.SaleZoneIds, "SaleZoneID");
            AddFilter(whereBuilder, filter.SupplierZoneIds, "SupplierZoneID");
        }
        private void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
            }
        }
        #endregion

    }
}


