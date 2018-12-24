using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
    public class CityDataManager : ICityDataManager
    {
        static string TABLE_NAME = "Common_City";
        static string TABLE_ALIAS = "vrCity";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_CountryID = "CountryID";
        const string COL_Settings = "Settings";
        const string COL_SourceId = "SourceId";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static CityDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_SourceId, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar,Size = 50 });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "common",
                DBTableName = "City",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }


        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        public City CityMapper(IRDBDataReader reader)
        {
            City city = new City
            {
                CityId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                CountryId = reader.GetInt(COL_CountryID),
                SourceId = reader.GetString(COL_SourceId),
                Settings = Vanrise.Common.Serializer.Deserialize<CitySettings>(reader.GetString(COL_Settings)),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
            };
            return city;
        }

        #endregion

        #region ICityDataManager
        public bool AreCitiesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<City> GetCities()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(CityMapper);
        }

        public bool Insert(City city, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(city.Name);
            ifNotExists.EqualsCondition(COL_CountryID).Value(city.CountryId);

            insertQuery.Column(COL_Name).Value(city.Name);
            insertQuery.Column(COL_CountryID).Value(city.CountryId);
            if (city.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(city.Settings));

            if (city.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(city.CreatedBy.Value);
            else
                insertQuery.Column(COL_CreatedBy).Null();

            if (city.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(city.LastModifiedBy.Value);
            else
                insertQuery.Column(COL_LastModifiedBy).Null();

            var insertedID = queryContext.ExecuteScalar().NullableIntValue;
            if(insertedID.HasValue)
            {
                insertedId = insertedID.Value;
                return true;
            }
            insertedId = -1;
            return false;
        }

        public bool Update(City city)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(city.CityId);
            ifNotExists.EqualsCondition(COL_Name).Value(city.Name);
            ifNotExists.EqualsCondition(COL_CountryID).Value(city.CountryId);

            updateQuery.Column(COL_Name).Value(city.Name);
            updateQuery.Column(COL_CountryID).Value(city.CountryId);

            if (city.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(city.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            if (city.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(city.LastModifiedBy.Value);
            else
                updateQuery.Column(COL_LastModifiedBy).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(city.CityId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

    }
}
