using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCountryDataManager : BaseSQLDataManager
    {
        public SourceCountryDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCountry> GetSourceCountries()
        {
            return GetItemsText(query_getSourceCountries, SourceCountryMapper, null);
        }

        private SourceCountry SourceCountryMapper(System.Data.IDataReader arg)
        {
            SourceCountry sourceCountry = new SourceCountry()
            {
                SourceId = arg["Code"].ToString(),
                Name = arg["Name"] as string,
            };
            return sourceCountry;
        }

        const string query_getSourceCountries = @"SELECT [Code] , [Name]  FROM [dbo].[CodeGroup] WITH (NOLOCK)";
    }
}
