using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	class VRTimeZoneDataManager : IVRTimeZoneDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRTimeZone";
		static string TABLE_ALIAS = "vrTimeZone";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Settings = "Settings";
		const string COL_SourceID = "SourceID";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedBy = "LastModifiedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRTimeZoneDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRTimeZone",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public List<VRTimeZone> GetVRTimeZones()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name,RDBSortDirection.ASC);
			return queryContext.GetItems(VRTimeZoneMapper);
		}

		public bool Insert(VRTimeZone timeZone, out int insertedId)
		{
			var queryCotext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryCotext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(timeZone.Name);
			insertQuery.AddSelectGeneratedId();
			insertQuery.Column(COL_Name).Value(timeZone.Name);
            if(timeZone.Settings != null)
		    	insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(timeZone.Settings));
			if (timeZone.CreatedBy.HasValue)
				insertQuery.Column(COL_CreatedBy).Value(timeZone.CreatedBy.Value);
			else
				insertQuery.Column(COL_CreatedBy).Null();
			if (timeZone.LastModifiedBy.HasValue)
				insertQuery.Column(COL_LastModifiedBy).Value(timeZone.LastModifiedBy.Value);
			else
				insertQuery.Column(COL_LastModifiedBy).Null();

			var	insertedID= queryCotext.ExecuteScalar().NullableIntValue;
			if (insertedID.HasValue)
            {
                insertedId = insertedID.Value;
                return true;
            }
            insertedId = -1;
            return false;
		}

		public bool Update(VRTimeZone timeZone)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(timeZone.TimeZoneId);
			ifNotExist.EqualsCondition(COL_Name).Value(timeZone.Name);
			updateQuery.Column(COL_Name).Value(timeZone.Name);
            if (timeZone.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(timeZone.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            if (timeZone.LastModifiedBy.HasValue)
				updateQuery.Column(COL_LastModifiedBy).Value(timeZone.LastModifiedBy.Value);
			else
				updateQuery.Column(COL_LastModifiedBy).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(timeZone.TimeZoneId);
			return queryContext.ExecuteNonQuery() > 0;

		}

		public bool AreVRTimeZonesUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private VRTimeZone VRTimeZoneMapper(IRDBDataReader reader)
		{

			return new VRTimeZone
			{
				TimeZoneId = reader.GetInt(COL_ID),
				Name = reader.GetString(COL_Name),
				Settings = Vanrise.Common.Serializer.Deserialize<VRTimeZoneSettings>(reader.GetString(COL_Settings)),
				CreatedTime = reader.GetDateTime(COL_CreatedTime),
				CreatedBy = reader.GetNullableInt(COL_CreatedBy),
				LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
				LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
			};
		}

		#endregion

	}




}

