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
            return new SourceSwitch()
            {
                SourceId = arg["SwitchID"].ToString(),
                Name = arg["Name"] as string,
                Configuration = arg["Configuration"] as string
            };
        }

        const string query_getSourceSwitches = @"SELECT [SwitchID] ,[Name], [Configuration] FROM [dbo].[Switch] WITH (NOLOCK)";
    }
}
