using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCarrierProfileDataManager : BaseSQLDataManager
    {
        public SourceCarrierProfileDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCarrierProfile> GetSourceCarrierProfiles()
        {
            return GetItemsText(query_getSourceCarrierProfiles, SourceCarrierProfileMapper, null);
        }

        private SourceCarrierProfile SourceCarrierProfileMapper(System.Data.IDataReader arg)
        {
            SourceCarrierProfile sourceCarrierProfile = new SourceCarrierProfile()
            {
                SourceId = arg["ProfileID"].ToString(),
                Name = arg["Name"].ToString(),
            };
            return sourceCarrierProfile;
        }

        const string query_getSourceCarrierProfiles = @"SELECT [ProfileID]  ,[Name]  FROM [dbo].[CarrierProfile] WITH (NOLOCK)";
    }
}
