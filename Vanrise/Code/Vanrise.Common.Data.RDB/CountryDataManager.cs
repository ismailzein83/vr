using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class CountryDataManager : ICountrytDataManager
	{

		static string TABLE_NAME = "common_Country";
		static string TABLE_ALIAS = "country";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_SourceID = "SourceID";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedBy = "LastModifiedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";


		static CountryDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "Country",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_Country", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion
		public List<Country> GetCountries()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(CountryMapper);
		}
		public bool Insert(Country country)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExists.EqualsCondition(COL_Name).Value(country.Name);
			insertQuery.Column(COL_ID).Value(country.CountryId);
			insertQuery.Column(COL_Name).Value(country.Name);
			if (country.CreatedBy.HasValue)
				insertQuery.Column(COL_CreatedBy).Value(country.CreatedBy.Value);
			else
				insertQuery.Column(COL_CreatedBy).Null();
			if (country.LastModifiedBy.HasValue)
				insertQuery.Column(COL_LastModifiedBy).Value(country.LastModifiedBy.Value);
			else
				insertQuery.Column(COL_LastModifiedBy).Null();
			return queryContext.ExecuteNonQuery() > 0;
		}
		public bool Update(Country country)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.NotEqualsCondition(COL_ID).Value(country.CountryId);
			ifNotExist.EqualsCondition(COL_Name).Value(country.Name);
			updateQuery.Column(COL_Name).Value(country.Name);
			if (country.LastModifiedBy.HasValue)
				updateQuery.Column(COL_LastModifiedBy).Value(country.LastModifiedBy.Value);
			else
				updateQuery.Column(COL_LastModifiedBy).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(country.CountryId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		public bool AreCountriesUpdated(ref object updateHandle)
		{
			throw new NotImplementedException();
		}
		public void InsertFromSource(Country country)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertFromSourceQuery = queryContext.AddInsertQuery();
			insertFromSourceQuery.IntoTable(TABLE_NAME);
			insertFromSourceQuery.Column(COL_ID).Value(country.CountryId);
			insertFromSourceQuery.Column(COL_Name).Value(country.Name);
			insertFromSourceQuery.Column(COL_SourceID).Value(country.SourceId);
			if (country.CreatedBy.HasValue)
				insertFromSourceQuery.Column(COL_CreatedBy).Value(country.CreatedBy.Value);
			else
				insertFromSourceQuery.Column(COL_CreatedBy).Null();
			if (country.LastModifiedBy.HasValue)
				insertFromSourceQuery.Column(COL_LastModifiedBy).Value(country.LastModifiedBy.Value);
			else
				insertFromSourceQuery.Column(COL_LastModifiedBy).Null();

			queryContext.ExecuteNonQuery();
		}
		public void UpdateFromSource(Country country)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateFromSourceQuery = queryContext.AddUpdateQuery();
			updateFromSourceQuery.FromTable(TABLE_NAME);
			updateFromSourceQuery.Column(COL_Name).Value(country.Name);
			if (country.LastModifiedBy.HasValue)
				updateFromSourceQuery.Column(COL_LastModifiedBy).Value(country.LastModifiedBy.Value);
			else
				updateFromSourceQuery.Column(COL_LastModifiedBy).Null();
			updateFromSourceQuery.Where().EqualsCondition(COL_ID).Value(country.CountryId);
			queryContext.ExecuteNonQuery();

		}
		#region Mappers
		private Country CountryMapper(IRDBDataReader reader)
		{
			Country country = new Country
			{
				CountryId = reader.GetInt(COL_ID),
				Name = reader.GetString(COL_Name),
				SourceId = reader.GetString(COL_SourceID),
				CreatedTime = reader.GetNullableDateTime(COL_CreatedTime),
				CreatedBy = reader.GetNullableInt(COL_CreatedBy),
				LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
				LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)

			};

			return country;
		}
		#endregion
	}
}
