using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
    public class NextBTableRouteDataManager : BaseSQLDataManager, INextBTableRouteDataManager
    {
        readonly string[] columns = { "OBA", "Prefix", "NextBTable" };

        public string SwitchId { get; set; }

        public NextBTableRouteDataManager() : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public void Initialize(INextBTableInitializeContext context)
        {
            string query = string.Format(query_CreateNextBTableByPrifxAndCustomerTable, SwitchId);
            ExecuteNonQueryText(query, null);
        }

        public void InsertNextBTables(IEnumerable<NextBTableDetails> bTables)
        {
            if (bTables != null && bTables.Any())
            {
                object dbApplyStream = InitialiazeStreamForDBApply();
                foreach (var bTable in bTables)
                {
                    WriteRecordToStream(bTable, dbApplyStream);
                }
                object obj = FinishDBApplyStream(dbApplyStream);
                ApplyRouteForDB(obj);
            }
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(NextBTableDetails record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.OBA, record.Prefix, record.NextBTable);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[NextBTable]", SwitchId),
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public void ApplyRouteForDB(object preparedRoute)
        {
            InsertBulkToTable(preparedRoute as BaseBulkInsertInfo);
        }

        public HashSet<int> GetAllNextBTables()
        {
            var nextBTables = new HashSet<int>();

            string query = string.Format(query_GetNextBTables.Replace("#FILTER#", ""), SwitchId);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    nextBTables.Add((int)reader["NextBTable"]);
                }
            }, null);

            return nextBTables;
        }

        public Dictionary<int, List<NextBTableDetails>> GetNextBTableDetailsByCustomerOBA()
        {
            var nextBTablesByCustomerOBA = new Dictionary<int, List<NextBTableDetails>>();

            string query = string.Format(query_GetNextBTablesByPrefixAndCustomer.Replace("#FILTER#", ""), SwitchId);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    NextBTableDetails nextBTableDetails = NextBTableDetailsMapper(reader);
                    var customerNextBTables = nextBTablesByCustomerOBA.GetOrCreateItem(nextBTableDetails.OBA);
                    customerNextBTables.Add(nextBTableDetails);
                }
            }, null);

            return nextBTablesByCustomerOBA;
        }
        
        NextBTableDetails NextBTableDetailsMapper(IDataReader reader)
        {
            return new NextBTableDetails()
            {
                OBA = (int)reader["OBA"],
                Prefix = reader["Prefix"] as string,
                NextBTable = (int)reader["NextBTable"],
            };
        }

        //new changes
        const string query_CreateNextBTableByPrifxAndCustomerTable = @"IF  NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.NextBTable') AND s.type in (N'U'))
		                                                    BEGIN
			                                                    CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[NextBTable](
                                                                    OBA int NOT NULL,
	                                                                Prefix varchar(20) NOT NULL,
	                                                                NextBTable int NOT NULL,
                                                                    CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.NextBTable] PRIMARY KEY CLUSTERED 
                                                                    (
	                                                                    OBA ASC,
	                                                                    Prefix ASC
                                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                                    ) ON [PRIMARY]
                                                                    
                                                                    CREATE NONCLUSTERED INDEX [IX_WhS_RouteSync_Ericsson_{0}.NextBTable_OBA] ON [WhS_RouteSync_Ericsson_{0}].[NextBTable]
														            (
															            [OBA] ASC
														            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

													        END";
        const string query_GetNextBTablesByPrefixAndCustomer = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.NextBTable') AND s.type in (N'U'))
                                                    SELECT btables.OBA, btables.Prefix, btables.NextBTable
                                                    FROM [WhS_RouteSync_Ericsson_{0}].[NextBTable] btables with(nolock) 
                                                    #FILTER#";

        const string query_GetNextBTables = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.NextBTable') AND s.type in (N'U'))
                                                    SELECT distinct btables.NextBTable
                                                    FROM [WhS_RouteSync_Ericsson_{0}].[NextBTable] btables with(nolock) 
                                                    #FILTER#";

    }
}
