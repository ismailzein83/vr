using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRLoggableEntityDataManager : IVRLoggableEntityDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "logging_LoggableEntity";
		static string TABLE_ALIAS = "vrLoggableEntity";
		const string COL_ID = "ID";
		const string COL_UniqueName = "UniqueName";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRLoggableEntityDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_UniqueName, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "logging",
				DBTableName = "LoggableEntity",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public Guid AddOrUpdateLoggableEntity(string entityUniqueName, VRLoggableEntitySettings loggableEntitySettings)
		{
			var queryContextToSelect = new RDBQueryContext(GetDataProvider());
			var queryContextToUpdate = new RDBQueryContext(GetDataProvider());
			var queryContextToInsert = new RDBQueryContext(GetDataProvider());


			var updateQuery = queryContextToUpdate.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(loggableEntitySettings));
			updateQuery.Where().EqualsCondition(TABLE_ALIAS, COL_UniqueName).Value(entityUniqueName);
			var effectedRows = queryContextToUpdate.ExecuteNonQuery();

			if (effectedRows <= 0)
			{
				try
				{
					var newId = Guid.NewGuid();
					var insertQuery = queryContextToInsert.AddInsertQuery();
					insertQuery.IntoTable(TABLE_NAME);
					var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
					ifNotExist.EqualsCondition(COL_UniqueName).Value(entityUniqueName);
				    insertQuery.Column(COL_ID).Value(newId);
					insertQuery.Column(COL_UniqueName).Value(entityUniqueName);
					insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(loggableEntitySettings));
					queryContextToInsert.ExecuteNonQuery();
					return newId;
				}
				catch (Exception ex){ }
			}
			var selectQuery=queryContextToSelect.AddSelectQuery();
			selectQuery.From(TABLE_NAME,TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().Column(COL_ID);
			selectQuery.Where().EqualsCondition(COL_UniqueName).Value(entityUniqueName);
			return queryContextToSelect.ExecuteScalar().GuidValue;
		}

		public bool AreVRObjectTrackingUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public List<VRLoggableEntity> GetAll()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(LoggableEntityMapper);
		}

		public string GenerateScript(List<VRLoggableEntity> loggableEntities, out string scriptEntityName)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("vrCommon", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private VRLoggableEntity LoggableEntityMapper(IRDBDataReader reader)
		{
			return new VRLoggableEntity
			{
				VRLoggableEntityId = reader.GetGuid(COL_ID),
				UniqueName = reader.GetString(COL_UniqueName),
				Settings = Vanrise.Common.Serializer.Deserialize<VRLoggableEntitySettings>(reader.GetString(COL_Settings))
			};
		}
		#endregion
	}
}
