using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class ServiceDataManager : BaseTOneDataManager, IServiceDataManager
    {
        public List<FlaggedService> GetServiceFlag()
        {
            return GetItemsSP("BEntity.sp_FlaggedService_GetServiceFlag", (reader) =>
                {
                    return new Entities.FlaggedService
                    {
                        FlaggedServiceID = (short)reader["FlaggedServiceID"],
                        Symbol = reader["Symbol"] as string
                    };
                });
        }
    }
}
