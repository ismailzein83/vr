﻿using System.Collections.Generic;
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

        private SourceCodeGroup SourceCodeGroupMapper(System.Data.IDataReader arg)
        {
            SourceCodeGroup sourceCountry = new SourceCodeGroup()
            {
                SourceId = arg["Code"].ToString(),
                Name = arg["Name"] as string,
            };
            return sourceCountry;
        }

        const string query_getSourceCodeGroups = @"SELECT [Code] , [Name]  FROM [dbo].[CodeGroup] WITH (NOLOCK)";
    }
}
