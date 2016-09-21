using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class CountryTOneV2SQLReader : SourceCountryReader
    {
        public override Guid ConfigId { get { return new Guid("5B63FFFD-8DD6-4365-9933-D62D1979E16E"); } }

        public string ConnectionString { get; set; }
        public override bool UseSourceItemId
        {
            get { return true; }
        }

        public override IEnumerable<SourceCountry> GetChangedItems(ref object updatedHandle)
        {
            DataManager datamanager = new DataManager(this.ConnectionString);
            return datamanager.GetUpdatedCountries(ref updatedHandle);
        }

        private class DataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            public DataManager(string connectionString)
                : base(connectionString, false)
            {
            }

            public List<SourceCountry> GetUpdatedCountries(ref object updateHandle)
            {
                return GetItemsText(query_getUpdatedZones, SourceZonerMapper, null);
            }

            private SourceCountry SourceZonerMapper(System.Data.IDataReader arg)
            {
                SourceCountry sourceZone = new SourceCountry()
                {
                    SourceId = arg["ID"].ToString(),
                    Name = arg["Name"] as string

                };
                return sourceZone;
            }

            const string query_getUpdatedZones = @"SELECT 
                [ID]
              ,[Name]
              FROM [common].[Country]";
        }
    }
}
