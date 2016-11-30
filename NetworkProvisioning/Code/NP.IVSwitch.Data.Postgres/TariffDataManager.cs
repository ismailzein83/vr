using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class TariffDataManager : BasePostgresDataManager
    {
        public TariffDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringTariffsKey", "NetworkProvisioningDBConnStringTariffs"))
        {

        }

        public int CreateTariffTable(int tariffId)
        {


            if (tariffId != -1)
            {
                // Create tariff table
                String[] cmdText4 = {
                                        string.Format(@"CREATE TABLE trf{0} (
                                                      dest_code character varying(30) NOT NULL,
                                                      time_frame character varying(50) NOT NULL,
                                                      dest_name character varying(100) DEFAULT NULL::character varying,
                                                      init_period integer,
                                                      next_period integer,
                                                      init_charge numeric(18,9) DEFAULT NULL::numeric,
                                                      next_charge numeric(18,9) DEFAULT NULL::numeric,
                                                      CONSTRAINT trf{0}_pkey PRIMARY KEY (dest_code, time_frame)
                                                    )
                                                    WITH (
                                                      OIDS=FALSE
                                                    );",tariffId.ToString()),
                                        string.Format("ALTER TABLE  trf{0}  OWNER TO zeinab;",tariffId.ToString()) 

                                    };

                ExecuteNonQuery(cmdText4);
                return tariffId;

            }

            return -1;
        }


        public void CreateGlobalTable()
        {

            String[] cmdText = { @"CREATE TABLE Global (
                                                      dest_code character varying(30) NOT NULL,
                                                      time_frame character varying(50) NOT NULL,
                                                      dest_name character varying(100) DEFAULT NULL::character varying,
                                                      init_period integer,
                                                      next_period integer,
                                                      init_charge numeric(18,9) DEFAULT NULL::numeric,
                                                      next_charge numeric(18,9) DEFAULT NULL::numeric,
                                                      CONSTRAINT Global_pkey PRIMARY KEY (dest_code, time_frame)
                                                    )
                                                    WITH (
                                                      OIDS=FALSE
                                                    );" ,
                                        "ALTER TABLE  Global  OWNER TO zeinab;",
                                       @"INSERT INTO Global (dest_code, time_frame,dest_name,init_period ,next_period,init_charge,next_charge) 
                                        SELECT  dest_code,time_frame,dest_name,init_period,next_period,init_charge,next_charge
                                        FROM  	unnest(ARRAY[0,1,2,3,4,5,6,7,8,9]) dest_code, 
                                                unnest(ARRAY['* * * * *']) time_frame,
                                                ARRAY_TO_STRING(ARRAY[NULL, dest_code],' ','GLobal ' ) dest_name,
                                                unnest(ARRAY[1]) init_period,unnest(ARRAY[1]) next_period,unnest(ARRAY[0.0]) init_charge,
                                                unnest(ARRAY[0.0]) next_charge" 
            };
               ExecuteNonQuery(cmdText);
        }
    }
}

