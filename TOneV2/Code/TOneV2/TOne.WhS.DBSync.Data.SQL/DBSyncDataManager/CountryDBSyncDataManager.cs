using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using System.Linq;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CountryDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Name", "SourceID", "ID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.Country);
        string _Schema = "Common";
        bool _UseTempTables;
        public CountryDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCountriesToTemp(List<Country> countries, int startingId)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in countries)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}", c.Name, c.SourceId, startingId++));
                }
                wr.Close();
            }

            Object preparedCountries = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCountries as BaseBulkInsertInfo);
        }

        public Dictionary<string, Country> GetCountries()
        {
            return GetItemsText("SELECT [ID] ,[Name] ,[SourceID] FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables), CountryMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
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
