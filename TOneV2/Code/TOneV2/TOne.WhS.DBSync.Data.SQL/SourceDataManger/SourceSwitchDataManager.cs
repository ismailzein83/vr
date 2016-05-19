using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceSwitchDataManager : BaseSQLDataManager
    {
        public SourceSwitchDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceSwitch> GetSourceSwitches()
        {
            return GetItemsText(query_getSourceSwitches, SourceSwitchMapper, null);
        }

        private SourceSwitch SourceSwitchMapper(IDataReader arg)
        {
            SourceSwitch sourceSwitch = new SourceSwitch()
            {
                SourceId = arg["SwitchID"].ToString(),
                Name = arg["Name"] as string,
            };
            return sourceSwitch;
        }

        const string query_getSourceSwitches = @"SELECT [SwitchID] ,[Name] FROM [dbo].[Switch] WITH (NOLOCK)";
    }
}
