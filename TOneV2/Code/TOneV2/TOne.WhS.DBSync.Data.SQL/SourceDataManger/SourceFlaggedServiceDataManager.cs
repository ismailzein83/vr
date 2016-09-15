using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceFlaggedServiceDataManager : BaseSQLDataManager
    {
         public SourceFlaggedServiceDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceFlaggedService> GetSourceFlaggedServices()
        {
            return GetItemsText(query_getSourceFlaggedServices, SourceFalggedServiceMapper, null);
        }

        private SourceFlaggedService SourceFalggedServiceMapper(IDataReader reader)
        {
            SourceFlaggedService sourceCarrierAccount = new SourceFlaggedService()
            {
                FlaggedServiceId = (short)reader["FlaggedServiceID"],
                Symbol = reader["Symbol"] as string,
                Name = reader["Name"] as string,
                Description = reader["Description"] as string,
                ServiceColor = reader["ServiceColor"] as string,
                SourceId = reader["FlaggedServiceID"].ToString()
            };
            return sourceCarrierAccount;
        }

        const string query_getSourceFlaggedServices = @"SELECT FlaggedServiceID, Symbol, Name, Description, ServiceColor
                                                                FROM  FlaggedService WITH (NOLOCK)";
    }
}
