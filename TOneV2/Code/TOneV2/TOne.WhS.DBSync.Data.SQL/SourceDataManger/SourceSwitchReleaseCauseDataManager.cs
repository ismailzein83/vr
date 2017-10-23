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
    public class SourceSwitchReleaseCauseDataManager : BaseSQLDataManager
    {
        public SourceSwitchReleaseCauseDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public IEnumerable<SourceSwitchReleaseCause> GetSourceSwitchReleaseCauses()
        {
            return GetItemsText(query_GetSourceSwitchReleaseCauses, SourceSwitchReleaseCauseMapper, null);
        }

        SourceSwitchReleaseCause SourceSwitchReleaseCauseMapper(IDataReader reader)
        {
            return new SourceSwitchReleaseCause
            {
                SourceId = reader["DS_ID_auto"].ToString(),
                Description = reader["Description"] as string,
                IsoCode = reader["IsoCode"] as string,
                ReleaseCode = reader["ReleaseCode"] as string,
                SwitchID = (byte)reader["SwitchID"],
                IsDelivered = reader["IsDelivered"] == DBNull.Value || reader["IsDelivered"].ToString().ToLower().Equals("n") ? false : true
            };
        }

        const string query_GetSourceSwitchReleaseCauses = @"
                                                            SELECT  [SwitchID]
                                                                  ,[ReleaseCode]
                                                                  ,[Description]
                                                                  ,[IsDelivered]
                                                                  ,[IsoCode]
                                                                  ,[DS_ID_auto]
                                                              FROM [dbo].[SwitchReleaseCode] with(nolock)";
    }
}
