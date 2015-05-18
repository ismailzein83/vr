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
    public class ServiceDataManager : BaseTOneDataManager, IServiceDataManager
    {
        public Dictionary<short, FlaggedService> GetServiceFlags()
        {
            Dictionary<short, FlaggedService> flaggedServices = new Dictionary<short, FlaggedService>();
            ExecuteReaderSP("BEntity.sp_FlaggedService_GetAll", (reader) =>
            {
                while (reader.Read())
                {
                    FlaggedService flaggedService = new FlaggedService()
                    {
                        FlaggedServiceID = (short)reader["FlaggedServiceID"],
                        Symbol = reader["Symbol"] as string
                    };
                    flaggedServices.Add(flaggedService.FlaggedServiceID, flaggedService);
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
                Symbol = reader["Symbol"] as string
            };
        }

        /// <summary>
        /// Get Matching Services
        /// </summary>
        /// <param name="serviceFlag">Service Flag combination ID</param>
        /// <returns>Matching Services comma separated</returns>
        public string GetServicesDisplayList(short serviceFlag)
        {
            IEnumerable<FlaggedService> flaggedServices = GetServiceFlags().Values;
            if (serviceFlag == 0) return GetServiceFlags()[0].Symbol;
            StringBuilder sb = new StringBuilder();
            foreach (FlaggedService service in flaggedServices)
                if ((service.FlaggedServiceID & serviceFlag) == service.FlaggedServiceID)
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(service.Symbol);
                }
            return sb.ToString();
        }

    }
}
