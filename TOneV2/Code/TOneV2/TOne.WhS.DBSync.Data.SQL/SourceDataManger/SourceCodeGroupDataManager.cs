using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCodeGroupDataManager : BaseSQLDataManager
    {
        public SourceCodeGroupDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCodeGroup> GetSourceCodeGroups()
        {
            return GetItemsText(query_getSourceCodeGroups, SourceCodeGroupMapper, null);
        }

        private SourceCodeGroup SourceCodeGroupMapper(IDataReader arg)
        {
            SourceCodeGroup sourceCodeGroup = new SourceCodeGroup()
            {
                SourceId = arg["Code"].ToString(),
                Name = arg["Name"] as string,
                Code = arg["Code"] as string
            };
            return sourceCodeGroup;
        }

        const string query_getSourceCodeGroups = @"SELECT [Code] , [Name]  FROM [dbo].[CodeGroup] WITH (NOLOCK) where Code <> '-'";
    }
}
