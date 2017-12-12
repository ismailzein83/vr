using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CountryDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.Country);
        string _Schema = "Common";
        bool _UseTempTables;
        public CountryDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCountriesToTemp(List<Country> countries, long startingId)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("ID", typeof(int));

            dt.BeginLoadData();
            foreach (var item in countries)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.Name;
                row[index++] = item.SourceId;
                row[index++] = startingId++;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, Country> GetCountries(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID] ,[Name] ,[SourceID] FROM {0} where sourceid is not null",
                MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), CountryMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public Country CountryMapper(IDataReader reader)
        {
            Country country = new Country
            {
                CountryId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SourceId = reader["SourceID"] as string,
            };

            return country;
        }

        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetTableName()
        {
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }
    }
}
