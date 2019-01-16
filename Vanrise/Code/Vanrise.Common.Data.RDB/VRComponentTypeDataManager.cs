using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRComponentTypeDataManager : IVRComponentTypeDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRComponentType";
		static string TABLE_ALIAS = "vrComponentType";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_ConfigID = "ConfigID";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRComponentTypeDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_ConfigID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRComponentType",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public bool AreVRComponentTypeUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public void GenerateScript(List<VRComponentType> componentTypes, Action<string, string> addEntityScript)
		{
			throw new NotImplementedException();
		}

		public List<VRComponentType> GetComponentTypes()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name,RDBSortDirection.ASC);
			return queryContext.GetItems(VRComponentTypeMapper);
		}

		public bool Insert(VRComponentType componentType)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);

			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(componentType.Name);

			insertQuery.Column(COL_ID).Value(componentType.VRComponentTypeId);
			insertQuery.Column(COL_Name).Value(componentType.Name);
			insertQuery.Column(COL_ConfigID).Value(componentType.Settings.VRComponentTypeConfigId);
			if(componentType.Settings!=null)
			insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(componentType.Settings));
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VRComponentType componentType)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);

			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(componentType.VRComponentTypeId);
			ifNotExist.EqualsCondition(COL_Name).Value(componentType.Name);

			updateQuery.Column(COL_Name).Value(componentType.Name);
			if (componentType.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(componentType.Settings));
			else
				updateQuery.Column(COL_Settings).Null();

			updateQuery.Column(COL_ConfigID).Value(componentType.Settings.VRComponentTypeConfigId);

			updateQuery.Where().EqualsCondition(COL_ID).Value(componentType.VRComponentTypeId);

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
		private VRComponentType VRComponentTypeMapper(IRDBDataReader reader)
		{
			return new VRComponentType
			{
				VRComponentTypeId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				Settings = Serializer.Deserialize<VRComponentTypeSettings>(reader.GetString(COL_Settings))
			};
		}
		#endregion

	}
}
