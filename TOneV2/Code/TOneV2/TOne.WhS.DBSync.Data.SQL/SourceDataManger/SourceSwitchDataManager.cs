using System.Collections.Generic;
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

        private SourceSwitch SourceSwitchMapper(System.Data.IDataReader arg)
        {
            SourceSwitch sourceSwitch = new SourceSwitch()
            {
                SourceId = arg["SwitchID"].ToString(),
                Name = arg["Name"].ToString()
            };
            return sourceSwitch;
        }

        const string query_getSourceSwitches = @"SELECT [SwitchID] ,[Name] FROM [dbo].[Switch]";
    }
}
