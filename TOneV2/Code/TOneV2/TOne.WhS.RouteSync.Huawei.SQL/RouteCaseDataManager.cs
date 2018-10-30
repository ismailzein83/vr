using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Huawei.SQL
{
    public class RouteCaseDataManager : BaseSQLDataManager, IRouteCaseDataManager
    {
        readonly string[] columns = { "RSName", "RouteCase" };

        public string SwitchId { get; set; }

        public RouteCaseDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public List<RouteCase> GetAllRouteCases()
        {
            List<RouteCase> routeCases = new List<RouteCase>();

            string query = string.Format(query_GetRouteCases.Replace("#FILTER#", ""), SwitchId);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    RouteCase routeCase = RouteCaseMapper(reader);
                    routeCases.Add(routeCase);
                }
            }, null);

            return routeCases.Count > 0 ? routeCases : null;
        }

        public List<RouteCase> GetNotSyncedRouteCases()
        {
            throw new NotImplementedException();
        }

        public void Initialize(IRouteCaseInitializeContext context)
        {
            string query = string.Format(query_CreateRouteCaseTable, SwitchId);
            ExecuteNonQueryText(query, null);
        }

        public Dictionary<string, RouteCase> GetRouteCasesAfterRCNumber(int rcNumber)
        {
            throw new NotImplementedException();
        }

        public void ApplyRouteCaseForDB(object preparedRouteCase)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(RouteCase record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}", record.RSName, record.RouteCaseAsString);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = string.Format("[WhS_RouteSync_Huawei_{0}].[RouteCase]", SwitchId),
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        private RouteCase RouteCaseMapper(IDataReader reader)
        {
            return new RouteCase()
            {
                RCNumber = (int)reader["RCNumber"],
                RSName = reader["RSName"] as string,
                RouteCaseAsString = reader["RouteCase"] as string
            };
        }

        public void UpdateSyncedRouteCases(IEnumerable<int> rcNumbers)
        {
            throw new NotImplementedException();
        }


        #region Queries

        const string query_CreateRouteCaseTable = @"IF NOT EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.RouteCase') AND s.type in (N'U'))
		                                            BEGIN
                                                        CREATE TABLE [WhS_RouteSync_Huawei_{0}].[RouteCase](
	                                                        [RCNumber] [int] IDENTITY(1,1) NOT NULL,
	                                                        [RSName] [varchar](max) NOT NULL,
	                                                        [RouteCase] [varchar](max) NOT NULL,
	                                                        [Synced] [bit] NOT NULL,
                                                         CONSTRAINT [PK_RouteCase] PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [RCNumber] ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                                                        CREATE NONCLUSTERED INDEX [WhS_RouteSync_Huawei_{0}.RouteCase_Synced] ON [WhS_RouteSync_Huawei_{0}].[RouteCase]
                                                        (
	                                                        [Synced] ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
		                                            END";

        const string query_GetRouteCases = @"SELECT rc.RCNumber, rc.RSName, rc.RouteCase
                                                    FROM [WhS_RouteSync_Huawei_{0}].[RouteCase] rc with(nolock) 
                                                    #FILTER#";

        #endregion
    }
}