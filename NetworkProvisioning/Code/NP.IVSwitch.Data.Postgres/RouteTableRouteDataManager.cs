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
        public List<RouteTableRoute> GetRouteTablesRoutes(int routeTableId, int limit, string aNumber, string bNumber)
        {
            string table = string.Format("rt{0}", routeTableId);
            String cmdText = string.Format(@"WITH codesTable AS ( SELECT distinct destination FROM {0} where ((destination is NULL or destination Like '%{2}%')and (tech_prefix is NULL or tech_prefix LIKE '%{3}%')) limit {1})
            SELECT rt.destination,route_id,routing_mode,preference,total_bkts,bkt_serial,bkt_capacity,bkt_tokens,flag_1,tech_prefix from {0} rt Join codesTable ct on ct.destination = rt.destination order by rt.destination;", table, limit, aNumber, bNumber);
            List<RouteTableRoute> routeTablesRoutes = new List<RouteTableRoute>();
            RouteTableRoute routeTableRoute = new Entities.RouteTableRoute.RouteTableRoute();
            routeTableRoute.RouteOptions = new List<RouteTableRouteOption>();
            string destination = "";
            string techPrefix = "";
            ExecuteReaderText(cmdText, (reader) =>
            {
                while (reader.Read())
                {

                    if (destination.Equals(""))
                    {
                        destination = GetReaderValue<string>(reader, "destination");
                        techPrefix = GetReaderValue<string>(reader, "tech_prefix");
                    }


                    if (destination != "" && GetReaderValue<string>(reader, "destination") != destination)
                    {
                        routeTableRoute.TechPrefix = techPrefix;
                        routeTableRoute.Destination = destination;
                        routeTablesRoutes.Add(routeTableRoute);
                        routeTableRoute = new RouteTableRoute();
                        destination = GetReaderValue<string>(reader, "destination");
                        techPrefix = GetReaderValue<string>(reader, "tech_prefix");
                        routeTableRoute.RouteOptions = new List<RouteTableRouteOption>();
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
                                Percentage = GetReaderValue<decimal>(reader, "flag_1")
                                

                          });}

                    }, null);

            routeTableRoute.Destination = destination;
            routeTableRoute.TechPrefix = techPrefix;
            routeTablesRoutes.Add(routeTableRoute);

            return routeTablesRoutes;
        }
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
                            cmdText1 = string.Format(@"INSERT INTO {0}(destination,route_id,time_frame,preference,huntstop,flag_1,routing_mode,total_bkts,bkt_serial,bkt_capacity,bkt_tokens,tech_prefix)
                         VALUES(@destination,@route_id,@time_frame,@preference,@huntstop,@percentage,@routingMode,@totalBKTs,@BKTSerial,@BKTCapacity,@BKTTokens,@techPrefix) ;", table);
                            rowCount = (int)ExecuteNonQueryText(cmdText1, (cmd) =>
                               {
                                   cmd.Parameters.AddWithValue("@destination", item.Destination);
                                   cmd.Parameters.AddWithValue("@route_id", routeOption.RouteId);
                                   cmd.Parameters.AddWithValue("@time_frame", "*****");
                                   cmd.Parameters.AddWithValue("@preference", routeOption.Preference);
                                   cmd.Parameters.AddWithValue("@percentage", routeOption.Percentage);
                                   cmd.Parameters.AddWithValue("@routingMode", routeOption.RoutingMode);
                                   cmd.Parameters.AddWithValue("@totalBKTs", routeOption.TotalBKTs);
                                   cmd.Parameters.AddWithValue("@BKTSerial", routeOption.BKTSerial);
                                   cmd.Parameters.AddWithValue("@BKTCapacity", routeOption.BKTCapacity);
                                   cmd.Parameters.AddWithValue("@BKTTokens", routeOption.BKTTokens);
                                   cmd.Parameters.AddWithValue("@techPrefix", (item.TechPrefix == null) ? (Object)DBNull.Value : item.TechPrefix);
                                   cmd.Parameters.AddWithValue("@huntstop",(routeOption.Huntstop==null)?(Object)DBNull.Value: routeOption.Huntstop);


                               });


                        }
                        else
                        {
                            cmdText1 = string.Format(@"INSERT INTO {0}(destination,route_id,time_frame,preference,huntstop,description,tech_prefix)
                            VALUES(@destination,@route_id,@time_frame,@preference,@huntstop,@description,@techPrefix);", table);

                            rowCount = (int)ExecuteNonQueryText(cmdText1, (cmd) =>
                            {
                                cmd.Parameters.AddWithValue("@destination", item.Destination);
                                cmd.Parameters.AddWithValue("@route_id", routeOption.RouteId);
                                cmd.Parameters.AddWithValue("@time_frame", "*****");
                                cmd.Parameters.AddWithValue("@preference", 0);
                                cmd.Parameters.AddWithValue("@description", "BLK");
                                   cmd.Parameters.AddWithValue("@techPrefix", (item.TechPrefix == null) ? (Object)DBNull.Value : item.TechPrefix);
                                cmd.Parameters.AddWithValue("@huntstop", (routeOption.Huntstop == null) ? (Object)DBNull.Value : routeOption.Huntstop);


                            });

                        }
                    }
                }
                transactionScope.Complete();
            }

            return true;



        }
        public RouteTableRoutesToEdit GetRouteTableRoutesOptions(int routeTableId, string destination)
        {
           
            string table = string.Format("rt{0}", routeTableId);
            String cmdText = string.Format(@"SELECT route_id,preference,huntstop,flag_1,tech_prefix from {0}  where destination=@destination;", table);
            RouteTableRoutesToEdit routeTableRoutesEditor = new RouteTableRoutesToEdit
            {
             
             RouteOptionsToEdit= new List<RouteTableRouteOptionToEdit>()
            };
            List<RouteTableRouteOptionToEdit> routeTableRouteOptionsToEdit = new List<RouteTableRouteOptionToEdit>();
            RouteTableRouteOptionToEdit routeTableRouteOptionToEdit = new RouteTableRouteOptionToEdit { 
            BackupOptions=new List<BackupOption>(),
            
            };
            List<BackupOption> backupOptions=new List<BackupOption>();
            bool firstRecordTechPrefixRead = false;
            bool firstRecordBackupRead = false;

            
            ExecuteReaderText(cmdText, (reader) =>
            {
               while(reader.Read())
                {
                  Int16? huntstop = GetReaderValue<Int16?>(reader, "huntstop");
                  int routeId=GetReaderValue<int>(reader, "route_id");
                  if (!firstRecordTechPrefixRead)
                  {
                      firstRecordTechPrefixRead = true;
                  routeTableRoutesEditor.TechPrefix = GetReaderValue<string>(reader, "tech_prefix");
                  }

                   if(huntstop==null)
                   {
                       routeTableRouteOptionToEdit=new RouteTableRouteOptionToEdit();
                       routeTableRouteOptionToEdit.RouteId=routeId;
                       routeTableRouteOptionToEdit.Preference=GetReaderValue<Int16>(reader, "preference");
                       routeTableRouteOptionToEdit.Percentage=GetReaderValue<decimal>(reader, "flag_1");
                       routeTableRouteOptionToEdit.TechPrefix=GetReaderValue<string>(reader, "tech_prefix");
                       routeTableRouteOptionsToEdit.Add(routeTableRouteOptionToEdit);


                   }
                   else
                       if (huntstop == 0 && !firstRecordBackupRead)
                       {
                           routeTableRouteOptionToEdit = new RouteTableRouteOptionToEdit
                           {

                               BackupOptions = new List<BackupOption>()
                           };
                           routeTableRouteOptionToEdit.RouteId = routeId;
                           firstRecordBackupRead = true;

                           routeTableRouteOptionToEdit.Preference = GetReaderValue<Int16>(reader, "preference");
                           routeTableRouteOptionToEdit.Percentage = GetReaderValue<decimal>(reader, "flag_1");
                           routeTableRouteOptionToEdit.TechPrefix = GetReaderValue<string>(reader, "tech_prefix");
                       }
                       else
                       if (huntstop!=null && firstRecordBackupRead)
                       {
                           backupOptions.Add(new BackupOption { 
                           BackupOptionRouteId=routeId
                           });
                       }
                       if (huntstop==1)
                       {
                           routeTableRouteOptionToEdit.BackupOptions = backupOptions;
                           routeTableRouteOptionsToEdit.Add(routeTableRouteOptionToEdit);
                           backupOptions = new List<BackupOption>();
                           firstRecordBackupRead = false;
                       }


               }

            }, (cmd) =>
          {

              cmd.Parameters.AddWithValue("@destination", destination);

          });
            routeTableRoutesEditor.Destination = destination;
            routeTableRoutesEditor.RouteOptionsToEdit = routeTableRouteOptionsToEdit;
            return routeTableRoutesEditor;
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
                if (routeTableRoute != null && routeTableRoute.RouteOptions!=null)
                foreach (var option in routeTableRoute.RouteOptions)
                {
                    String cmdText1 = "";
                    int rowCount;
                    if (IsBlockedAccount == false)
                    {
                        cmdText1 = string.Format(@"INSERT INTO {0}(destination,route_id,time_frame,preference,huntstop,flag_1,routing_mode,total_bkts,bkt_serial,bkt_capacity,bkt_tokens,tech_prefix)
                         VALUES(@destination,@route_id,@time_frame,@preference,@huntstop,@percentage,@routingMode,@totalBKTs,@BKTSerial,@BKTCapacity,@BKTTokens,@techPrefix) ;", table);
                        rowCount = (int)ExecuteNonQueryText(cmdText1, (cmd) =>
                        {
                            cmd.Parameters.AddWithValue("@destination", routeTableRoute.Destination);
                            cmd.Parameters.AddWithValue("@route_id", option.RouteId);
                            cmd.Parameters.AddWithValue("@time_frame", "*****");
                            cmd.Parameters.AddWithValue("@preference", option.Preference);
                            cmd.Parameters.AddWithValue("@percentage", option.Percentage);
                            cmd.Parameters.AddWithValue("@routingMode", option.RoutingMode);
                            cmd.Parameters.AddWithValue("@totalBKTs", option.TotalBKTs);
                            cmd.Parameters.AddWithValue("@BKTSerial", option.BKTSerial);
                            cmd.Parameters.AddWithValue("@BKTCapacity", option.BKTCapacity);
                            cmd.Parameters.AddWithValue("@BKTTokens", option.BKTTokens);
                            cmd.Parameters.AddWithValue("@techPrefix", (routeTableRoute.TechPrefix == null) ? (Object)DBNull.Value : routeTableRoute.TechPrefix);
                            cmd.Parameters.AddWithValue("@huntstop", (option.Huntstop == null) ? (Object)DBNull.Value : option.Huntstop);



                        });
                    }
                    else
                    {
                        cmdText1 = string.Format(@"INSERT INTO {0}(destination,route_id,time_frame,preference,huntstop,description,tech_prefix)
                            VALUES(@destination,@route_id,@time_frame,@preference,@huntstop,@description,@techPrefix);", table);

                        rowCount = (int)ExecuteNonQueryText(cmdText1, (cmd) =>
                        {
                            cmd.Parameters.AddWithValue("@destination", routeTableRoute.Destination);
                            cmd.Parameters.AddWithValue("@route_id", option.RouteId);
                            cmd.Parameters.AddWithValue("@time_frame", "*****");
                            cmd.Parameters.AddWithValue("@preference", -1);
                            cmd.Parameters.AddWithValue("@description", "BLK");
                            cmd.Parameters.AddWithValue("@techPrefix", (routeTableRoute.TechPrefix == null) ? (Object)DBNull.Value : routeTableRoute.TechPrefix);
                            cmd.Parameters.AddWithValue("@huntstop", (option.Huntstop == null) ? (Object)DBNull.Value : option.Huntstop);


                        });

                    }
                }

                transactionScope.Complete();
            }
            return true;


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
            if (percentage != DBNull.Value)
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
