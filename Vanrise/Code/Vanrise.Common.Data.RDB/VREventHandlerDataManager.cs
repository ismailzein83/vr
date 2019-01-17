using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VREventHandlerDataManager: IVREventHandlerDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VREventHandler";
		static string TABLE_ALIAS = "vrEventHandler";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Settings = "Settings";
		const string COL_BED = "BED";
		const string COL_EED = "EED";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion


		#region Constructors
		static VREventHandlerDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VREventHandler",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public List<VREventHandler> GetVREventHandlers()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(VREventHandlerMapper);
		}

		public bool AreVREventHandlerUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public bool Insert(VREventHandler vREventHandler)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(vREventHandler.Name);
			insertQuery.Column(COL_ID).Value(vREventHandler.VREventHandlerId);
			insertQuery.Column(COL_Name).Value(vREventHandler.Name);
			if(vREventHandler.Settings!=null)
			insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vREventHandler.Settings));
			insertQuery.Column(COL_BED).Value(vREventHandler.BED);
			if (vREventHandler.EED.HasValue)
				insertQuery.Column(COL_EED).Value(vREventHandler.EED.Value);
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VREventHandler vREventHandler)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(vREventHandler.VREventHandlerId);
			ifNotExist.EqualsCondition(COL_Name).Value(vREventHandler.Name);
			updateQuery.Column(COL_Name).Value(vREventHandler.Name);
			if (vREventHandler.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vREventHandler.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Column(COL_BED).Value(vREventHandler.BED);
			if (vREventHandler.EED.HasValue)
				updateQuery.Column(COL_EED).Value(vREventHandler.EED.Value);
			else
				updateQuery.Column(COL_EED).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(vREventHandler.VREventHandlerId);
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
		private VREventHandler VREventHandlerMapper(IRDBDataReader reader)
		{
			return new VREventHandler
			{
				VREventHandlerId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				Settings = Vanrise.Common.Serializer.Deserialize<VREventHandlerSettings>(reader.GetString(COL_Settings)),
				BED = reader.GetDateTime(COL_BED),
				EED = reader.GetNullableDateTime(COL_EED),
			};
		}
		#endregion

	}
}
