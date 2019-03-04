using NP.IVSwitch.Entities.RouteTableRoute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;
using TOne.WhS.BusinessEntity.Entities;
using System.Transactions;
using Vanrise.Common;
using NP.IVSwitch.Entities;


namespace NP.IVSwitch.Data.Postgres
{
	public class RouteTableRouteDataManager : BasePostgresDataManager, IRouteTableRouteDataManager
	{
		public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }

		protected override string GetConnectionString()
		{
			return IvSwitchSync.RouteConnectionString;
		}
		#region Public Methods

		public int CreateRouteTable(int routeId)
		{
			if (routeId == -1) return -1;

			String[] cmdText = {
				string.Format(@"CREATE TABLE rt{0} (
                                                      destination character varying(20) NOT NULL,
                                                      route_id integer NOT NULL,
                                                      time_frame character varying(50) NOT NULL,
                                                      preference smallint NOT NULL,
                                                      huntstop smallint,
                                                      huntstop_rc character varying(50) DEFAULT NULL::character varying,
                                                      min_profit numeric(5,5) DEFAULT NULL::numeric,
                                                      state_id smallint,
                                                      wakeup_time timestamp without time zone,
                                                      description character varying(50),
                                                      routing_mode integer DEFAULT 1,
                                                      total_bkts integer DEFAULT 1,
                                                      bkt_serial integer DEFAULT 1,
                                                      bkt_capacity integer DEFAULT 1,
                                                      bkt_tokens integer DEFAULT 1,
                                                      p_score integer DEFAULT 0,
                                                      flag_1 numeric DEFAULT 1,
                                                      flag_2 numeric DEFAULT 1,
                                                      flag_3 integer DEFAULT 1,
                                                      flag_4 integer DEFAULT 1,
                                                      flag_5 numeric DEFAULT 1,
                                                      tech_prefix character varying(20),
                                                      CONSTRAINT rt{0}_pkey PRIMARY KEY (destination, route_id, time_frame, preference)
                                                    )
                                                    WITH (
                                                      OIDS=FALSE
                                                    );",routeId.ToString()),
				string.Format("ALTER TABLE  rt{0}  OWNER TO {1};",routeId.ToString(),IvSwitchSync.OwnerName)

			};
			ExecuteNonQuery(cmdText);
			return routeId;
		}
		public bool InsertHelperRoute(int routeId, string description)
		{
			string query = @"INSERT INTO ui_helper_routes(route_id, description)
	                            SELECT @route_id, @description WHERE( NOT EXISTS( SELECT 1 FROM ui_helper_routes WHERE route_id = @route_id ));";
			int recordsEffected = ExecuteNonQueryText(query, cmd =>
			{
				cmd.Parameters.AddWithValue("@route_id", routeId);
				cmd.Parameters.AddWithValue("@description", description);
			});
			return recordsEffected > 0;
		}
		public bool DeleteHelperRoute(int routeId)
		{
			String cmdDeleteHelperRoute = string.Format("delete from ui_helper_routes where route_id={0}", routeId);
			int effectedRows = ExecuteNonQueryText(cmdDeleteHelperRoute, cmd => { });
			return effectedRows > 0;
		}
		public void CreateRouteTableRoute(int routeTableId)
		{
			string table = string.Format("rt{0}", routeTableId);
			string cmdText = string.Format(@"CREATE TABLE public.{0}
                            (
                            destination character varying(20) COLLATE pg_catalog.""default"" NOT NULL,
                            route_id integer NOT NULL,
                            time_frame character varying(50) COLLATE pg_catalog.""default"" NOT NULL,
                            preference smallint NOT NULL,
                            huntstop smallint,
                            huntstop_rc character varying(50) COLLATE pg_catalog.""default"" DEFAULT NULL::character varying,
                            min_profit numeric(5, 5) DEFAULT NULL::numeric,
                            state_id smallint,
                            wakeup_time timestamp without time zone,
                            description character varying(50) COLLATE pg_catalog.""default"",
                            routing_mode integer DEFAULT 1,
                            total_bkts integer DEFAULT 1,
                            bkt_serial integer DEFAULT 1,
                            bkt_capacity integer DEFAULT 1,
                            bkt_tokens integer DEFAULT 1,
                            p_score integer DEFAULT 0,
                            flag_1 numeric DEFAULT 0,
                            flag_2 numeric DEFAULT 0,
                            flag_3 integer DEFAULT 0,
                            flag_4 integer DEFAULT 0,
                            flag_5 numeric DEFAULT 0,
                            tech_prefix character varying(20) COLLATE pg_catalog.""default"",
                            CONSTRAINT {0}_temp_pkey PRIMARY KEY (destination, route_id, time_frame, preference)
                            )
                            WITH (
                                OIDS = FALSE
                            )
                            TABLESPACE pg_default;
                            ALTER TABLE public.{0}
                            OWNER to {1};", table, IvSwitchSync.OwnerName);
			int tabl = ExecuteNonQueryText(cmdText, cmd => { });
		}
		public List<RouteTableRoute> GetRouteTablesRoutes(RouteTableViewType routeTableViewType, int routeTableId, int limit, string aNumber, string bNumber,string whitelist, List<int> routeIds)
		{
			string table = string.Format("rt{0}", routeTableId);
			string cmdText = "";
			string search = "";
			switch (routeTableViewType)
			{
				case RouteTableViewType.ANumber:
					search = string.Format("((destination is NULL or destination Like '%{0}%') {1} and (tech_prefix is NULL or tech_prefix LIKE '%{2}%'))", aNumber, routeIds == null ? "" : string.Format(" and route_id in({0})", string.Join<int>(",", routeIds)), bNumber);
					break;
				case RouteTableViewType.Whitelist:
					search = string.Format("((destination is NULL or destination Like '%{0}%') {1})", whitelist, routeIds == null ? "" : string.Format(" and route_id in({0})", string.Join<int>(",", routeIds)));
					break;
				case RouteTableViewType.BNumber:
					search = string.Format("((destination is NULL or destination Like '%{0}%') {1})", bNumber, routeIds == null ? "" : string.Format(" and route_id in({0})", string.Join<int>(",", routeIds)));
					break;
			}
			cmdText = string.Format(@"WITH codesTable AS ( SELECT distinct destination FROM {0} where {2} limit {1})
                     SELECT rt.destination,route_id,routing_mode,preference,huntstop,total_bkts,bkt_serial,bkt_capacity,bkt_tokens,flag_1,tech_prefix from {0} rt Join codesTable ct on ct.destination = rt.destination order by rt.destination;", table, limit,search );



			List<RouteTableRoute> routeTablesRoutes = new List<RouteTableRoute>();
			RouteTableRoute routeTableRoute = new Entities.RouteTableRoute.RouteTableRoute();
			routeTableRoute.RouteOptions = new List<RouteTableRouteOption>();
			string destination = "";
			string techPrefix = "";
			string preDestination = null;
			string nextDestination = null;
			ExecuteReaderText(cmdText, (reader) =>
			{
				while (reader.Read())
				{
					nextDestination = GetReaderValue<string>(reader, "destination");
					if (nextDestination != destination)
					{
						preDestination = destination;
						destination = nextDestination;
						techPrefix = GetReaderValue<string>(reader, "tech_prefix");
						routeTableRoute = new RouteTableRoute()
						{
							TechPrefix = techPrefix,
							Destination = destination,
							RouteOptions = new List<RouteTableRouteOption>()
						};
						routeTablesRoutes.Add(routeTableRoute);
					}
					routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
					{

						RouteId = GetReaderValue<int>(reader, "route_id"),
						RoutingMode = GetReaderValue<int>(reader, "routing_mode"),
						Preference = GetReaderValue<Int16>(reader, "preference"),
						TotalBKTs = GetReaderValue<int>(reader, "total_bkts"),
						BKTSerial = GetReaderValue<int>(reader, "bkt_serial"),
						BKTCapacity = GetReaderValue<int>(reader, "bkt_capacity"),
						BKTTokens = GetReaderValue<int>(reader, "bkt_tokens"),
						Percentage = GetReaderValue<decimal?>(reader, "flag_1"),
						Huntstop=GetReaderValue<Int16>(reader,"huntstop")
					});
				}

			}, null);
			return routeTablesRoutes;
		}
		//public RouteTableRoutesToEdit GetRouteTableRoutesOptions(int routeTableId, string destination)
		//{

		//    string table = string.Format("rt{0}", routeTableId);
		//    String cmdText = string.Format(@"SELECT route_id,preference,huntstop,flag_1,tech_prefix from {0}  where destination=@destination order by preference DESC;", table);
		//    RouteTableRoutesToEdit routeTableRoutesEditor = new RouteTableRoutesToEdit
		//    {
		//        RouteOptionsToEdit = new List<RouteTableRouteOptionToEdit>()
		//    };
		//    //  List<RouteTableRouteOptionToEdit> routeTableRouteOptionsToEdit = new List<RouteTableRouteOptionToEdit>();
		//    var routeTableRouteOptionToEdit = new RouteTableRouteOptionToEdit
		//    {
		//        BackupOptions = new List<BackupOption>(),
		//    };
		//    bool filledFirstItem = false;
		//    bool firstTechPrefixReaded = false;

		//    ExecuteReaderText(cmdText, (reader) =>
		//    {
		//        while (reader.Read())
		//        {
		//            Int16? huntstop = GetReaderValue<Int16?>(reader, "huntstop");
		//            int routeId = GetReaderValue<int>(reader, "route_id");
		//            if (!firstTechPrefixReaded)
		//            {
		//                routeTableRoutesEditor.TechPrefix = GetReaderValue<string>(reader, "tech_prefix");
		//                firstTechPrefixReaded = true;
		//            }
		//            if (!filledFirstItem)
		//            {
		//                routeTableRouteOptionToEdit.RouteId = routeId;
		//                routeTableRouteOptionToEdit.Preference = GetReaderValue<Int16>(reader, "preference");
		//                routeTableRouteOptionToEdit.Percentage = GetReaderValue<decimal>(reader, "flag_1");
		//                routeTableRouteOptionToEdit.TechPrefix = GetReaderValue<string>(reader, "tech_prefix");
		//                filledFirstItem = true;
		//                if (huntstop.HasValue)
		//                {
		//                    if (huntstop.Value == 1)
		//                    {
		//                        filledFirstItem = false;
		//                        routeTableRoutesEditor.RouteOptionsToEdit.Add(routeTableRouteOptionToEdit);
		//                        routeTableRouteOptionToEdit = new RouteTableRouteOptionToEdit
		//                        {
		//                            BackupOptions = new List<BackupOption>(),
		//                        };
		//                    }
		//                }
		//                continue;
		//            }

		//            if (huntstop.HasValue)
		//            {
		//                if (huntstop.Value == 1)
		//                {
		//                    routeTableRouteOptionToEdit.BackupOptions.Add(new BackupOption
		//                    {
		//                        BackupOptionRouteId = routeId
		//                    });
		//                    routeTableRoutesEditor.RouteOptionsToEdit.Add(routeTableRouteOptionToEdit);
		//                    filledFirstItem = false;
		//                    routeTableRouteOptionToEdit = new RouteTableRouteOptionToEdit
		//                    {
		//                        BackupOptions = new List<BackupOption>(),
		//                    };
		//                }
		//                else
		//                {
		//                    routeTableRouteOptionToEdit.BackupOptions.Add(new BackupOption
		//                    {
		//                        BackupOptionRouteId = routeId
		//                    });
		//                }
		//            }
		//        }
		//    }, (cmd) =>
		//    {

		//        cmd.Parameters.AddWithValue("@destination", destination);

		//    });
		//    routeTableRoutesEditor.Destination = destination;
		//    //routeTableRoutesEditor.RouteOptionsToEdit = routeTableRouteOptionsToEdit;
		//    return routeTableRoutesEditor;
		//}
		public bool Insert(List<RouteTableRoute> routeTableRoutes, int routeTableId, bool IsBlockedAccount)
		{
			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};

			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{

				foreach (var item in routeTableRoutes)
				{
					string table = string.Format("rt{0}", routeTableId);
					string cmdText = string.Format("DELETE FROM {0} where destination=@destination;", table);
					int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
					{
						cmd.Parameters.AddWithValue("@destination", item.Destination);

					});

					foreach (var routeOption in item.RouteOptions)
					{
						String cmdText1 = "";
						int rowCount;
						if (IsBlockedAccount == false)
						{
							cmdText1 = string.Format(@"INSERT INTO {0}(destination,route_id,time_frame,preference,huntstop,description,state_id,flag_1,routing_mode,total_bkts,bkt_serial,bkt_capacity,bkt_tokens,tech_prefix,huntstop_rc, min_profit, wakeup_time)
                         VALUES(@destination,@route_id,@time_frame,@preference,@huntstop,@description,@stateId,@percentage,@routingMode,@totalBKTs,@BKTSerial,@BKTCapacity,@BKTTokens,@techPrefix, @huntstop_rc, @min_profit, @wakeup_time) ;", table);
							rowCount = (int)ExecuteNonQueryText(cmdText1, (cmd) =>
							   {
								   cmd.Parameters.AddWithValue("@destination", item.Destination);
								   cmd.Parameters.AddWithValue("@route_id", routeOption.RouteId);
								   cmd.Parameters.AddWithValue("@time_frame", "* * * * *");
								   cmd.Parameters.AddWithValue("@preference", routeOption.Preference);
								   cmd.Parameters.AddWithValue("@percentage", (routeOption.Percentage == null) ? 0 : routeOption.Percentage);
								   cmd.Parameters.AddWithValue("@routingMode", routeOption.RoutingMode);
								   cmd.Parameters.AddWithValue("@totalBKTs", routeOption.TotalBKTs);
								   cmd.Parameters.AddWithValue("@BKTSerial", routeOption.BKTSerial);
								   cmd.Parameters.AddWithValue("@BKTCapacity", routeOption.BKTCapacity);
								   cmd.Parameters.AddWithValue("@BKTTokens", routeOption.BKTTokens);
								   cmd.Parameters.AddWithValue("@techPrefix", (item.TechPrefix == null) ? "" : item.TechPrefix);
								   cmd.Parameters.AddWithValue("@huntstop", (routeOption.Huntstop == null) ? (Object)DBNull.Value : routeOption.Huntstop);
								   cmd.Parameters.AddWithValue("@stateId", routeOption.StateId);
								   cmd.Parameters.AddWithValue("@description", routeOption.Description);
								   cmd.Parameters.AddWithValue("@huntstop_rc", "");
								   cmd.Parameters.AddWithValue("@min_profit", 0);
								   cmd.Parameters.AddWithValue("@wakeup_time", GetCurrentDate());

							   });
						}
						else
						{
							cmdText1 = string.Format(@"INSERT INTO {0}(destination,route_id,time_frame,preference,huntstop,state_id,description,tech_prefix,huntstop_rc, min_profit, wakeup_time)
                            VALUES(@destination,@route_id,@time_frame,@preference,@huntstop,@stateId,@description,@techPrefix, @huntstop_rc, @min_profit, @wakeup_time);", table);

							rowCount = (int)ExecuteNonQueryText(cmdText1, (cmd) =>
							{
								cmd.Parameters.AddWithValue("@destination", item.Destination);
								cmd.Parameters.AddWithValue("@route_id", routeOption.RouteId);
								cmd.Parameters.AddWithValue("@time_frame", "* * * * *");
								cmd.Parameters.AddWithValue("@preference", 0);
								cmd.Parameters.AddWithValue("@description", "BLK");
								cmd.Parameters.AddWithValue("@techPrefix", (item.TechPrefix == null) ? "" : item.TechPrefix);
								cmd.Parameters.AddWithValue("@huntstop", 1);
								cmd.Parameters.AddWithValue("@stateId", 1);
								cmd.Parameters.AddWithValue("@huntstop_rc", "");
								cmd.Parameters.AddWithValue("@min_profit", 0);
								cmd.Parameters.AddWithValue("@wakeup_time", GetCurrentDate());

							});

						}
					}
				}
				transactionScope.Complete();
			}

			return true;



		}
		public bool Update(RouteTableRoute routeTableRoute, int routeTableId, bool IsBlockedAccount)
		{

			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};

			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				string table = string.Format("rt{0}", routeTableId);
				string cmdText = string.Format("DELETE FROM {0} where destination=@destination;", table);
				int recordsEffected = (int)ExecuteNonQueryText(cmdText, cmd =>
				{
					cmd.Parameters.AddWithValue("@destination", routeTableRoute.Destination);

				});
				if (routeTableRoute != null && routeTableRoute.RouteOptions != null)
					foreach (var option in routeTableRoute.RouteOptions)
					{
						String cmdText1 = "";
						int rowCount;
						if (IsBlockedAccount == false)
						{
							cmdText1 = string.Format(@"INSERT INTO {0}(destination,route_id,time_frame,preference,huntstop,description,state_id,flag_1,routing_mode,total_bkts,bkt_serial,bkt_capacity,bkt_tokens,tech_prefix, huntstop_rc, min_profit, wakeup_time)
                         VALUES(@destination,@route_id,@time_frame,@preference,@huntstop,@description,@stateId,@percentage,@routingMode,@totalBKTs,@BKTSerial,@BKTCapacity,@BKTTokens,@techPrefix, @huntstop_rc, @min_profit, @wakeup_time) ;", table);
							rowCount = (int)ExecuteNonQueryText(cmdText1, (cmd) =>
							{
								cmd.Parameters.AddWithValue("@destination", routeTableRoute.Destination);
								cmd.Parameters.AddWithValue("@route_id", option.RouteId);
								cmd.Parameters.AddWithValue("@time_frame", "* * * * *");
								cmd.Parameters.AddWithValue("@preference", option.Preference);
								cmd.Parameters.AddWithValue("@percentage", (option.Percentage == null) ? 0 : option.Percentage);
								cmd.Parameters.AddWithValue("@routingMode", option.RoutingMode);
								cmd.Parameters.AddWithValue("@totalBKTs", option.TotalBKTs);
								cmd.Parameters.AddWithValue("@BKTSerial", option.BKTSerial);
								cmd.Parameters.AddWithValue("@BKTCapacity", option.BKTCapacity);
								cmd.Parameters.AddWithValue("@BKTTokens", option.BKTTokens);
								cmd.Parameters.AddWithValue("@techPrefix", (routeTableRoute.TechPrefix == null) ? "" : routeTableRoute.TechPrefix);
								cmd.Parameters.AddWithValue("@huntstop", (option.Huntstop == null) ? (Object)DBNull.Value : option.Huntstop);
								cmd.Parameters.AddWithValue("@stateId", option.StateId);
								cmd.Parameters.AddWithValue("@description", option.Description);
								cmd.Parameters.AddWithValue("@huntstop_rc", "");
								cmd.Parameters.AddWithValue("@min_profit", 0);
								cmd.Parameters.AddWithValue("@wakeup_time", GetCurrentDate());



							});
						}
						else
						{
							cmdText1 = string.Format(@"INSERT INTO {0}(destination,route_id,time_frame,preference,huntstop,state_id,description,tech_prefix, huntstop_rc, min_profit, wakeup_time)
                            VALUES(@destination,@route_id,@time_frame,@preference,@huntstop,@stateId,@description,@techPrefix, @huntstop_rc, @min_profit, @wakeup_time);", table);

							rowCount = (int)ExecuteNonQueryText(cmdText1, (cmd) =>
							{
								cmd.Parameters.AddWithValue("@destination", routeTableRoute.Destination);
								cmd.Parameters.AddWithValue("@route_id", option.RouteId);
								cmd.Parameters.AddWithValue("@time_frame", "* * * * *");
								cmd.Parameters.AddWithValue("@preference", 0);
								cmd.Parameters.AddWithValue("@description", "BLK");
								cmd.Parameters.AddWithValue("@techPrefix", (routeTableRoute.TechPrefix == null) ? "" : routeTableRoute.TechPrefix);
								cmd.Parameters.AddWithValue("@huntstop",1);
								cmd.Parameters.AddWithValue("@stateId", 1);
								cmd.Parameters.AddWithValue("@huntstop_rc", "");
								cmd.Parameters.AddWithValue("@min_profit", 0);
								cmd.Parameters.AddWithValue("@wakeup_time", GetCurrentDate());


							});

						}
					}

				transactionScope.Complete();
			}
			return true;


		}
		public DateTime GetCurrentDate()
		{
			string query = "select current_timestamp;";
			return (DateTime)ExecuteScalarText(query, null);
		}
		public bool DropRouteTableRoute(int routeTableId)
		{
			string table = string.Format("rt{0}", routeTableId);
			string cmdText = string.Format("DROP TABLE {0} ;", table);
			int recordsEffected = ExecuteNonQueryText(cmdText, null);
			return true;

		}
		public bool DeleteRouteTableRoutes(int routeTableId, string destination)
		{
			string table = string.Format("rt{0}", routeTableId);
			string cmdText = string.Format("DELETE FROM {0} where destination=@destination;", table);
			int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
			{

				cmd.Parameters.AddWithValue("@destination", destination);
			});

			return true;



		}
		public RouteTableRoutesToEdit GetRouteTableRoutesOptions(int routeTableId, string destination, int blockedAccountId)
		{
			string table = string.Format("rt{0}", routeTableId);
			String cmdText = string.Format(@"SELECT route_id,preference,huntstop,routing_mode,flag_1,tech_prefix from {0}  where destination=@destination order by preference DESC;", table);
			RouteTableRoutesToEdit routeTableRoutesEditor = new RouteTableRoutesToEdit
			{
				RouteOptionsToEdit = new List<RouteTableRouteOptionToEdit>()
			};
			RouteTableRouteOptionToEdit routeTableRouteOptionToEdit = null;
			bool firstTechPrefixReaded = false;
			decimal percentage = 0;
			ExecuteReaderText(cmdText, (reader) =>
			{
				while (reader.Read())
				{
					if (!firstTechPrefixReaded)
					{
						routeTableRoutesEditor.TechPrefix = GetReaderValue<string>(reader, "tech_prefix");
						firstTechPrefixReaded = true;
						percentage = GetReaderValue<decimal>(reader, "flag_1");
					}

					var routingMode = GetReaderValue<int>(reader, "routing_mode");
					var routeId = GetReaderValue<int>(reader, "route_id");
					if (routingMode == 1 && routeId == blockedAccountId)
					{
						routeTableRouteOptionToEdit = new RouteTableRouteOptionToEdit();
						routeTableRouteOptionToEdit.RouteId = routeId;
						routeTableRouteOptionToEdit.Preference = GetReaderValue<Int16>(reader, "preference");
						routeTableRouteOptionToEdit.Percentage = GetReaderValue<decimal>(reader, "flag_1");
						routeTableRouteOptionToEdit.TechPrefix = GetReaderValue<string>(reader, "tech_prefix");
						routeTableRoutesEditor.IsBlockedAccount = true;
						routeTableRoutesEditor.RouteOptionsToEdit.Add(routeTableRouteOptionToEdit);
						break;
					}
					if (percentage > 0)
					{
						if (routingMode == 8)
						{
							if (routeTableRouteOptionToEdit != null)
							{
								routeTableRoutesEditor.RouteOptionsToEdit.Add(routeTableRouteOptionToEdit);
							}
							routeTableRouteOptionToEdit = new RouteTableRouteOptionToEdit
							{
								BackupOptions = new List<BackupOption>(),
							};
							routeTableRouteOptionToEdit.RouteId = routeId;
							routeTableRouteOptionToEdit.Preference = GetReaderValue<Int16>(reader, "preference");
							routeTableRouteOptionToEdit.Percentage = GetReaderValue<decimal>(reader, "flag_1");
							routeTableRouteOptionToEdit.TechPrefix = GetReaderValue<string>(reader, "tech_prefix");
							routeTableRouteOptionToEdit.BackupOptions = new List<BackupOption>();
						}
						else
						{
							routeTableRouteOptionToEdit.BackupOptions.Add(new BackupOption() { BackupOptionRouteId = routeId });
						}

					}
					else
					{
						routeTableRouteOptionToEdit = new RouteTableRouteOptionToEdit();
						routeTableRouteOptionToEdit.RouteId = routeId;
						routeTableRouteOptionToEdit.Preference = GetReaderValue<Int16>(reader, "preference");
						routeTableRouteOptionToEdit.Percentage = GetReaderValue<decimal>(reader, "flag_1");
						routeTableRouteOptionToEdit.TechPrefix = GetReaderValue<string>(reader, "tech_prefix");
						routeTableRoutesEditor.RouteOptionsToEdit.Add(routeTableRouteOptionToEdit);
					}
				}
				if (routeTableRouteOptionToEdit != null && percentage>0)
					routeTableRoutesEditor.RouteOptionsToEdit.Add(routeTableRouteOptionToEdit);
			}, (cmd) =>
			{
				cmd.Parameters.AddWithValue("@destination", destination);
			});
			routeTableRoutesEditor.Destination = destination;
			return routeTableRoutesEditor;
		}
		public bool CheckIfCodesExist(List<string> codes, int routeTableId)
		{
			string table = string.Format("rt{0}", routeTableId);
			string a = string.Format("('{0}')", string.Join<string>("','", codes));
			string query = string.Format("SELECT count(*)  FROM {0} WHERE destination in {1}", table, string.Format("('{0}')", string.Join<string>("','", codes)));
			object count = ExecuteScalarText(query, cmd =>
			{
			});
			int result = Convert.ToInt32(count);
			return result > 0;
		}

		#endregion
		#region mappers

		private RouteTableRouteOptionToEdit RoutePreferenceMapper(IDataReader reader)
		{
			RouteTableRouteOptionToEdit routePreference = new RouteTableRouteOptionToEdit();

			var routeId = reader["route_id"];
			var preference = reader["preference"];
			var percentage = reader["flag_1"];
			var techPrefix = reader["tech_prefix"];

			if (routeId != DBNull.Value)
			{
				routePreference.RouteId = (int)routeId;
			}
			if (preference != DBNull.Value)
			{
				routePreference.Preference = (Int16)preference;
			}
			if (percentage != DBNull.Value && (int)percentage != 0)
			{
				routePreference.Percentage = (decimal)percentage;
			}
			if (techPrefix != DBNull.Value)
			{
				routePreference.TechPrefix = (string)techPrefix;
			}

			return routePreference;
		}

		#endregion

	}
}
