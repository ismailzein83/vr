using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
    public class CodeGroupRouteDataManager : BaseSQLDataManager, ICodeGroupRouteDataManager
    {
        const string CodeGroupRouteTableName = "CodeGroupRoute";

        readonly string[] columns = { "BO", "Code", "RCNumber" };

        public string SwitchId { get; set; }

        public CodeGroupRouteDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public void Initialize(ICodeGroupRouteInitializeContext context)
        {
            Guid guid = Guid.NewGuid();
            string createTableQuery = string.Format(query_CreateCodeGroupRouteTable, SwitchId, guid, CodeGroupRouteTableName);
            ExecuteNonQueryText(createTableQuery, null);
        }

        public void InsertRoutes(IEnumerable<CodeGroupRoute> routes)
        {
            if (routes != null && routes.Any())
            {
                object dbApplyStream = InitialiazeStreamForDBApply();
                foreach (var route in routes)
                {
                    WriteRecordToStream(route, dbApplyStream);
                }
                object obj = FinishDBApplyStream(dbApplyStream);
                ApplyRouteForDB(obj);
            }
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[{1}]", SwitchId, CodeGroupRouteTableName),
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(CodeGroupRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.BO, record.CodeGroup, record.RCNumber);
        }

        public void ApplyRouteForDB(object preparedRoute)
        {
            InsertBulkToTable(preparedRoute as BaseBulkInsertInfo);
        }

        public Dictionary<int, List<CodeGroupRoute>> GetFilteredCodeGroupRouteByBO(IEnumerable<int> customerBOs)
        {
            var convertedRoutesByBO = new Dictionary<int, List<CodeGroupRoute>>();

            string filter = "";

            if (customerBOs != null && customerBOs.Any())
                filter = string.Format(" Where BO in ({0})", string.Join(",", customerBOs));

            string query = string.Format(query_GetFilteredRoute.Replace("#FILTER#", filter), SwitchId, CodeGroupRouteTableName);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    var convertedRoute = CodeGroupRouteMapper(reader);
                    List<CodeGroupRoute> convertedRoutes = convertedRoutesByBO.GetOrCreateItem(convertedRoute.BO);
                    convertedRoutes.Add(convertedRoute);
                }
            }, null);
            return convertedRoutesByBO;
        }

        CodeGroupRoute CodeGroupRouteMapper(IDataReader reader)
        {
            return new CodeGroupRoute()
            {
                BO = (int)reader["BO"],
                CodeGroup = reader["Code"] as string,
                RCNumber = (int)reader["RCNumber"]
            };
        }

        #region Queries

        const string query_CreateCodeGroupRouteTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                        BEGIN
                                                            DROP TABLE WhS_RouteSync_Ericsson_{0}.{2}
                                                        END

                                                        CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}](
                                                              BO int NOT NULL,
	                                                          Code varchar(20) NOT NULL,
	                                                          RCNumber int NOT NULL
                                                        CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.CodeGroupRoute_{2}{1}] PRIMARY KEY CLUSTERED 
                                                        (
                                                            BO ASC,
	                                                        Code ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
													    ) ON [PRIMARY]";

        const string query_GetFilteredRoute = @"Select BO,Code,RCNumber
                                                FROM [WhS_RouteSync_Ericsson_{0}].[{1}]
                                                #FILTER#";

        #endregion
    }
}