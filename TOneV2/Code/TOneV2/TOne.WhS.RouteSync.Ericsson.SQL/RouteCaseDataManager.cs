﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
	public class RouteCaseDataManager : BaseSQLDataManager, IRouteCaseDataManager
	{
		readonly string[] columns = { "RCNumber", "Options" };

		public string SwitchId { get; set; }

		public RouteCaseDataManager()
			: base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
		{

		}

		public void Initialize(IRouteCaseInitializeContext context)
		{
			var blockedRCOption = new List<RouteCaseOption>() { Helper.GetBlockedRouteCaseOption() };
			var branchRoutes = context.BranchRouteSettings.GenerateBranchRoutes(new GenerateBranchRoutesContext() { RouteCaseOptions = blockedRCOption });
			var serializedBlockedRCOption = Helper.SerializeRouteCase(blockedRCOption, branchRoutes);
			string query = string.Format(query_CreateRouteCaseTable, SwitchId, context.FirstRCNumber, serializedBlockedRCOption);
			ExecuteNonQueryText(query, null);
		}

		public object InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}

		public void WriteRecordToStream(RouteCase record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.WriteRecord("{0}^{1}", record.RCNumber, record.RouteCaseAsString);
		}

		public void ApplyRouteCaseForDB(object preparedRouteCase)
		{
			InsertBulkToTable(preparedRouteCase as BaseBulkInsertInfo);
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new StreamBulkInsertInfo
			{
				TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[RouteCase]", SwitchId),
				Stream = streamForBulkInsert,
				TabLock = true,
				KeepIdentity = false,
				FieldSeparator = '^',
				ColumnNames = columns
			};
		}

		public Dictionary<string, RouteCase> GetRouteCasesAfterRCNumber(int routeCaseNumber)
		{
			Dictionary<string, RouteCase> routeCases = new Dictionary<string, RouteCase>();

			string query = string.Format(query_GetRouteCases.Replace("#FILTER#", " WHERE rc.RCNumber > {1}"), SwitchId, routeCaseNumber);
			ExecuteReaderText(query, (reader) =>
			{
				while (reader.Read())
				{
					RouteCase routeCase = RouteCaseMapper(reader);
					routeCases.Add(routeCase.RouteCaseAsString, routeCase);
				}
			}, null);

			return routeCases;
		}

		public void UpdateSyncedRouteCases(IEnumerable<int> rCNumbers)
		{
			if (rCNumbers == null || !rCNumbers.Any())
				return;
			string filter = string.Format(" Where RCNumber in ({0})", string.Join(",", rCNumbers));
			string query = string.Format(query_UpdateSyncedRouteCases.Replace("#FILTER#", filter), SwitchId);
			ExecuteNonQueryText(query, null);
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

			return routeCases;
		}

		public List<RouteCase> GetNotSyncedRouteCases()
		{
			List<RouteCase> routeCases = new List<RouteCase>();

			string query = string.Format(query_GetRouteCases.Replace("#FILTER#", " WHERE Synced = 0 "), SwitchId);
			ExecuteReaderText(query, (reader) =>
			{
				while (reader.Read())
				{
					RouteCase routeCase = RouteCaseMapper(reader);
					routeCases.Add(routeCase);
				}
			}, null);

			return routeCases;
		}

		RouteCase RouteCaseMapper(IDataReader reader)
		{
			return new RouteCase()
			{
				RCNumber = (int)reader["RCNumber"],
				RouteCaseAsString = reader["Options"] as string,
			};
		}

		const string query_GetRouteCases = @"SELECT rc.RCNumber, rc.Options, rc.Synced
                                                    FROM [WhS_RouteSync_Ericsson_{0}].[RouteCase] rc with(nolock) 
                                                    #FILTER#";

		const string query_CreateRouteCaseTable = @"IF  NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.RouteCase') AND s.type in (N'U'))
		                                            begin
			                                            CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[RouteCase](
	                                                        [RCNumber] [int] NOT NULL,
	                                                        [Options] [varchar](max) NOT NULL,
															[Synced] bit NOT NULL DEFAULT 0,
                                                         CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.RouteCase] PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [RCNumber] ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                        ) ON [PRIMARY];
														
														CREATE NONCLUSTERED INDEX [IX_WhS_RouteSync_Ericsson_{0}.RouteCase_Synced] ON [WhS_RouteSync_Ericsson_{0}].[RouteCase]
														(
															[Synced] ASC
														)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

		                                            END
													IF  NOT EXISTS( SELECT * FROM WhS_RouteSync_Ericsson_{0}.[RouteCase] WHERE RCNumber = {1})
		                                            begin
														INSERT INTO  WhS_RouteSync_Ericsson_{0}.[RouteCase] (RCNumber, Options) 
														VALUES ({1}, '{2}');
													END";

		const string query_UpdateSyncedRouteCases = @"UPDATE [WhS_RouteSync_Ericsson_{0}].[RouteCase]
														SET [Synced] = 1
														#FILTER#";
	}
}