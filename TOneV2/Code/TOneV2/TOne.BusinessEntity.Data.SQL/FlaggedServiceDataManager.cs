using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class FlaggedServiceDataManager : BaseTOneDataManager, IFlaggedServiceDataManager
    {
        public Dictionary<short, FlaggedService> GetServiceFlags()
        {
            short i = 0;
            Dictionary<short, FlaggedService> flaggedServices = new Dictionary<short, FlaggedService>();
            ExecuteReaderSP("BEntity.sp_FlaggedService_GetAll", (reader) =>
            {
                while (reader.Read())
                {
                    FlaggedService flaggedService = FlaggedServiceMapper(reader);
                    flaggedServices.Add(i, flaggedService);
                    i++;
                }
            });

            return flaggedServices;
        }

        public FlaggedService GetServiceFlag(short id)
        {
            return GetItemSP("BEntity.sp_FlaggedService_GetById", FlaggedServiceMapper, id);
        }

        private FlaggedService FlaggedServiceMapper(IDataReader reader)
        {

            return new FlaggedService()
            {
                FlaggedServiceID = (short)reader["FlaggedServiceID"],
                Symbol = reader["Symbol"] as string,
                ServiceColor = reader["ServiceColor"] as string
            };
        }
    }
}
