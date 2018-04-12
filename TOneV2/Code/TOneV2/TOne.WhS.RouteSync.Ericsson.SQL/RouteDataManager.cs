using System;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
    public class RouteDataManager : BaseSQLDataManager, IRouteDataManager
    {
        public string SwitchId { get; set; }
        public RouteDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public void Initialize(IRouteInitializeContext context)
        {
            Guid guid = Guid.NewGuid();
            string query = string.Format(query_CreateRouteTempTable, SwitchId, guid);
            ExecuteNonQueryText(query, null);
        }

        public void Finalize(IRouteFinalizeContext context)
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(ConvertedRoute record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        const string query_CreateRouteTempTable = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.Route_Temp') AND s.type in (N'U'))
                                                          begin
                                                              DROP TABLE WhS_RouteSync_Ericsson_{0}.Route_Temp
                                                          end
                                                          
                                                          CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[Route_temp](
                                                                BO varchar(255) NOT NULL,
	                                                            Code varchar(20) NOT NULL,
	                                                            RCNumber int NOT NULL
                                                          CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.Route_{1}] PRIMARY KEY CLUSTERED 
                                                         (
                                                             BO ASC,
	                                                         Code ASC
                                                         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                         ) ON [PRIMARY]";
    }
}