using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRConnectionDataManager: IVRConnectionDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_Connection";
		static string TABLE_ALIAS = "connection";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		#endregion
		
		#region Contructors
		static VRConnectionDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "Connection",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods

		public List<VRConnection> GetVRConnections()
		{
			var queryConext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryConext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_Settings);
			return queryConext.GetItems(VRConnectionMapper);
		}
		public bool Update(VRConnection connection)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.EqualsCondition(COL_Name).Value(connection.Name);
			ifNotExist.NotEqualsCondition(COL_ID).Value(connection.VRConnectionId);
			updateQuery.Column(COL_Name).Value(connection.Name);
			updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(connection.Settings));
			updateQuery.Where().EqualsCondition(COL_ID).Value(connection.VRConnectionId);
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Insert(VRConnection connection)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(connection.Name);
			insertQuery.Column(COL_ID).Value(connection.VRConnectionId);
			insertQuery.Column(COL_Name).Value(connection.Name);
			insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(connection.Settings));
			return queryContext.ExecuteNonQuery() > 0;
		}



		public bool AreVRConnectionsUpdated(ref object updateHandle)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_Connection", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private VRConnection VRConnectionMapper(IRDBDataReader reader)
		{
			return new VRConnection
			{
				VRConnectionId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				Settings = Vanrise.Common.Serializer.Deserialize<VRConnectionSettings>(reader.GetString(COL_Settings))
			};
		}
		#endregion
	}
}
