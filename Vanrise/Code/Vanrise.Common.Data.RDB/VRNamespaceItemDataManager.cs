using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRNamespaceItemDataManager : IVRNamespaceItemDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRNamespaceItem";
		static string TABLE_ALIAS = "vrNamespaceItem";
		const string COL_ID = "ID";
		const string COL_VRNamespaceId = "VRNamespaceId";
		const string COL_Name = "Name";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		const string COL_LastModifiedBy = "LastModifiedBy";
		#endregion

		#region Constructors
		static VRNamespaceItemDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_VRNamespaceId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRNamespaceItem",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public bool AreVRNamespaceItemUpdated(ref object lastReceivedDataInfo)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
		}

		public List<VRNamespaceItem> GetVRNamespaceItems()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(VRNamespaceItemMapper);
		}

		public bool Insert(VRNamespaceItem vrNamespaceItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);

			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(vrNamespaceItem.Name);

			insertQuery.Column(COL_ID).Value(vrNamespaceItem.VRNamespaceItemId);
			insertQuery.Column(COL_VRNamespaceId).Value(vrNamespaceItem.VRNamespaceId);
			insertQuery.Column(COL_Name).Value(vrNamespaceItem.Name);

			if (vrNamespaceItem.Settings != null)
			insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrNamespaceItem.Settings));

			insertQuery.Column(COL_CreatedBy).Value(vrNamespaceItem.CreatedBy);
			insertQuery.Column(COL_LastModifiedBy).Value(vrNamespaceItem.LastModifiedBy);

			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VRNamespaceItem vrNamespaceItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);

			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(vrNamespaceItem.VRNamespaceItemId);
			ifNotExist.EqualsCondition(COL_Name).Value(vrNamespaceItem.Name);

			updateQuery.Column(COL_VRNamespaceId).Value(vrNamespaceItem.VRNamespaceId);
			updateQuery.Column(COL_Name).Value(vrNamespaceItem.Name);

			if (vrNamespaceItem.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrNamespaceItem.Settings));
			else
				updateQuery.Column(COL_Settings).Null();

			updateQuery.Column(COL_LastModifiedBy).Value(vrNamespaceItem.LastModifiedBy);
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrNamespaceItem.VRNamespaceItemId);

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
		private VRNamespaceItem VRNamespaceItemMapper(IRDBDataReader reader)
		{
			return new VRNamespaceItem
			{
				VRNamespaceItemId = reader.GetGuid(COL_ID),
				VRNamespaceId = reader.GetGuid(COL_VRNamespaceId),
				Name = reader.GetString(COL_Name) as string,
				Settings = Vanrise.Common.Serializer.Deserialize<VRNamespaceItemSettings>(reader.GetString(COL_Settings)),
				CreatedBy = reader.GetInt(COL_CreatedBy),
				LastModifiedBy = reader.GetInt(COL_LastModifiedBy)
			};
		}
		#endregion


	}
}
