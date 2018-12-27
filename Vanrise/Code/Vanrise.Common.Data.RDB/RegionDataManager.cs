﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class RegionDataManager: IRegionDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_Region";
		static string TABLE_ALIAS = "vrRegion";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_CountryID = "CountryID";
		const string COL_Settings = "Settings";
		const string COL_SourceID = "SourceID";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedBy = "LastModifiedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static RegionDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "Region",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreRegionsUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

		public List<Region> GetRegions()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(RegionMapper);
		}

		public bool Insert(Region region, out int insertedId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(region.Name);
			ifNotExist.EqualsCondition(COL_CountryID).Value(region.CountryId);
			insertQuery.AddSelectGeneratedId();
			insertQuery.Column(COL_Name).Value(region.Name);
			insertQuery.Column(COL_CountryID).Value(region.CountryId);
            if(region.Settings != null)
			  insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(region.Settings));
			if (region.CreatedBy.HasValue)
				insertQuery.Column(COL_CreatedBy).Value(region.CreatedBy.Value);
			else
				insertQuery.Column(COL_CreatedBy).Null();
			if (region.LastModifiedBy.HasValue)
				insertQuery.Column(COL_LastModifiedBy).Value(region.LastModifiedBy.Value);
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

		public bool Update(Region region)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(region.RegionId);
			ifNotExist.EqualsCondition(COL_Name).Value(region.Name);
			ifNotExist.EqualsCondition(COL_CountryID).Value(region.CountryId);
			updateQuery.Column(COL_Name).Value(region.Name);
			updateQuery.Column(COL_CountryID).Value(region.CountryId);
            if (region.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(region.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            if (region.LastModifiedBy.HasValue)
				updateQuery.Column(COL_LastModifiedBy).Value(region.LastModifiedBy.Value);
			else
				updateQuery.Column(COL_LastModifiedBy).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(region.RegionId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers

		Region RegionMapper(IRDBDataReader reader)
		{
			var settings = reader.GetString(COL_Settings);

			return new Region()
			{

				RegionId = reader.GetInt(COL_ID),
				Name = reader.GetString(COL_Name),
				CountryId = (int)reader.GetInt(COL_CountryID),
				Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<RegionSettings>(settings) : null,
				CreatedTime = reader.GetDateTime(COL_CreatedTime),
				CreatedBy = reader.GetNullableInt(COL_CreatedBy),
				LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
				LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)

			};
		}

		#endregion

	}

}
