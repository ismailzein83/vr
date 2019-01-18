using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class EntityPersonalizationDataManager : IEntityPersonalizationDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_EntityPersonalization";
		static string TABLE_ALIAS = "vrEntityPersonalization";
		const string COL_ID = "ID";
		const string COL_UserID = "UserID";
		const string COL_EntityUniqueName = "EntityUniqueName";
		const string COL_Details = "Details";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		const string COL_LastModifiedBy = "LastModifiedBy";
		#endregion

		#region Constructors
		static EntityPersonalizationDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_EntityUniqueName, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 1000 });
			columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "EntityPersonalization",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public bool AreEntityPersonalizationUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public bool Delete(long entityPersonalizationId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var deleteQuery = queryContext.AddDeleteQuery();
			deleteQuery.FromTable(TABLE_NAME);
			deleteQuery.Where().EqualsCondition(COL_ID).Value(entityPersonalizationId);
			return queryContext.ExecuteNonQuery() > 0;
		}

		public List<EntityPersonalization> GetEntityPersonalizations()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(EntityPersonalizationMapper);
		}

		public bool Save(EntityPersonalization entityPersonalization)
		{
			var queryContextToUpdate = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContextToUpdate.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);

			if (entityPersonalization.Setting != null)
				updateQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(entityPersonalization.Setting));
			else
				updateQuery.Column(COL_Details).Null();

			if (entityPersonalization.CreatedBy.HasValue)
				updateQuery.Column(COL_LastModifiedBy).Value(entityPersonalization.CreatedBy.Value);
			else
				updateQuery.Column(COL_LastModifiedBy).Null();

			var whereContextConditions = updateQuery.Where();
			whereContextConditions.EqualsCondition(COL_EntityUniqueName).Value(entityPersonalization.EntityUniqueName);
			if (entityPersonalization.UserId.HasValue)
			{
				whereContextConditions.EqualsCondition(COL_UserID).Value(entityPersonalization.UserId.Value);
			}
			else
			{
				whereContextConditions.NullCondition().Column(COL_UserID);
			}
			var effectedRows = queryContextToUpdate.ExecuteNonQuery();
			if (effectedRows > 0)
				return true;


			var queryContextToInsert = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContextToInsert.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);

			var ifNotExistInsertContext = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExistInsertContext.EqualsCondition(TABLE_ALIAS, COL_EntityUniqueName).Value(entityPersonalization.EntityUniqueName);
			if (entityPersonalization.UserId.HasValue)
			{
				ifNotExistInsertContext.EqualsCondition(TABLE_ALIAS, COL_UserID).Value(entityPersonalization.UserId.Value);
			}
			else
			{
				ifNotExistInsertContext.NullCondition().Column(COL_UserID);
			}

			if (entityPersonalization.UserId.HasValue)
				insertQuery.Column(COL_UserID).Value(entityPersonalization.UserId.Value);

			insertQuery.Column(COL_EntityUniqueName).Value(entityPersonalization.EntityUniqueName);

			if (entityPersonalization.Setting != null)
				insertQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(entityPersonalization.Setting));

			if (entityPersonalization.CreatedBy.HasValue)
				insertQuery.Column(COL_CreatedBy).Value(entityPersonalization.CreatedBy.Value);

			if (entityPersonalization.CreatedBy.HasValue)
				insertQuery.Column(COL_LastModifiedBy).Value(entityPersonalization.CreatedBy.Value);
			return queryContextToInsert.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private EntityPersonalization EntityPersonalizationMapper(IRDBDataReader reader)
		{
			return new EntityPersonalization
			{
				EntityPersonalizationId = reader.GetLong(COL_ID),
				UserId = reader.GetNullableInt(COL_UserID),
				EntityUniqueName = reader.GetString(COL_EntityUniqueName),
				Setting = reader.GetString(COL_Details) != null ? Vanrise.Common.Serializer.Deserialize<EntityPersonalizationExtendedSetting>(reader.GetString(COL_Details)) : null,
				CreatedTime = reader.GetDateTime(COL_CreatedTime),
				CreatedBy = reader.GetNullableInt(COL_CreatedBy),
				LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
				LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime),
			};
		}
		#endregion
	}
}
