using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRActionAuditLKUPDataManager : IVRActionAuditLKUPDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "logging_ActionAuditLKUP";
		static string TABLE_ALIAS = "vrLoggingActionAuditLKUP";
		const string COL_ID = "ID";
		const string COL_Type = "Type";
		const string COL_Name = "Name";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRActionAuditLKUPDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Type, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "logging",
				DBTableName = "ActionAuditLKUP",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public int AddLKUPIfNotExists(VRActionAuditLKUPType lkupType, string name)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_NAME);
			ifNotExist.EqualsCondition(COL_Type).Value((int)lkupType);
			ifNotExist.EqualsCondition(COL_Name).Value(name);

			insertQuery.Column(COL_Type).Value((int)lkupType);
			insertQuery.Column(COL_Name).Value(name);

			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Column(COL_ID);
			selectQuery.Where().EqualsCondition(COL_Type).Value((int)lkupType);
			selectQuery.Where().EqualsCondition(COL_Name).Value(name);
			return queryContext.ExecuteScalar().IntValue;
		}

		public bool AreVRActionAuditLKUPUpdated(ref object lastReceivedDataInfo)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
		}

		public List<VRActionAuditLKUP> GetAll()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(VRActionAuditLKUPMapper);
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "LoggingConfigDBConnStringKey", "LoggingConfigDBConnString");
		}
		#endregion

		#region Mappers
		private VRActionAuditLKUP VRActionAuditLKUPMapper(IRDBDataReader reader)
		{
			return new VRActionAuditLKUP
			{
				VRActionAuditLKUPId = reader.GetInt(COL_ID),
				Type = (VRActionAuditLKUPType)reader.GetInt(COL_Type),
				Name = reader.GetString(COL_Name)
			};
		}
		#endregion

	}
}
