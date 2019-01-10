using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRActionAuditDataManager : IVRActionAuditDataManager
	{

		#region Local Variables
		static string TABLE_NAME = "logging_ActionAudit";
		static string TABLE_ALIAS = "vrLoggingActionAudit";
		const string COL_ID = "ID";
		const string COL_UserID = "UserID";
		const string COL_URLID = "URLID";
		const string COL_ModuleID = "ModuleID";
		const string COL_EntityID = "EntityID";
		const string COL_ActionID = "ActionID";
		const string COL_ObjectID = "ObjectID";
		const string COL_ObjectName = "ObjectName";
		const string COL_ObjectTrackingID = "ObjectTrackingID";
		const string COL_ActionDescription = "ActionDescription";
		const string COL_LogTime = "LogTime";
		const string COL_CreatedTime = "CreatedTime";
		#endregion

		#region Contructors
		static VRActionAuditDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_URLID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_ModuleID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_EntityID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_ActionID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_ObjectID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_ObjectName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 900 });
			columns.Add(COL_ObjectTrackingID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_ActionDescription, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_LogTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "logging",
				DBTableName = "ActionAudit",
				Columns = columns,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods
		public List<VRActionAudit> GetFilterdActionAudits(VRActionAuditQuery query)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, query.TopRecord);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			var whereCondition = selectQuery.Where();
			if (query.UserIds != null && query.UserIds.Count > 0)
				whereCondition.ListCondition(COL_UserID, RDBListConditionOperator.IN, query.UserIds);

			if (query.ModuleIds != null && query.ModuleIds.Count > 0)
				whereCondition.ListCondition(COL_ModuleID, RDBListConditionOperator.IN, query.ModuleIds);

			if (query.ActionIds != null && query.ActionIds.Count > 0)
				whereCondition.ListCondition(COL_ActionID, RDBListConditionOperator.IN, query.ActionIds);

			if (query.EntityIds != null && query.EntityIds.Count > 0)
				whereCondition.ListCondition(COL_EntityID, RDBListConditionOperator.IN, query.EntityIds);

			if (string.IsNullOrEmpty(query.ObjectName))
				whereCondition.ContainsCondition(COL_ObjectName, query.ObjectName);

			if (string.IsNullOrEmpty(query.ObjectId))
				whereCondition.EqualsCondition(TABLE_ALIAS, COL_ObjectName).Value(query.ObjectId);

			whereCondition.GreaterOrEqualCondition(TABLE_ALIAS, COL_LogTime).Value(query.FromTime);

			if (query.ToTime.HasValue)
				whereCondition.LessOrEqualCondition(TABLE_ALIAS, COL_LogTime).Value(query.ToTime.Value);

			selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);
			return queryContext.GetItems(ActionAuditMapper);
		}

		public void Insert(int? userId, int? urlId, int moduleId, int entityId, int actionId, string objectId, string objectName, long? objectTrackingId, string actionDescription)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			if (userId.HasValue)
				insertQuery.Column(COL_UserID).Value(userId.Value);
			if (urlId.HasValue)
				insertQuery.Column(COL_URLID).Value(urlId.Value);
			insertQuery.Column(COL_ModuleID).Value(moduleId);
			insertQuery.Column(COL_EntityID).Value(entityId);
			insertQuery.Column(COL_ActionID).Value(actionId);
			insertQuery.Column(COL_ObjectID).Value(objectId);
			insertQuery.Column(COL_ObjectName).Value(objectName);
			insertQuery.Column(COL_ActionDescription).Value(objectId);
			if (objectTrackingId.HasValue)
				insertQuery.Column(COL_ObjectTrackingID).Value(objectTrackingId.Value);
			insertQuery.Column(COL_LogTime).DateNow();
			queryContext.ExecuteNonQuery();
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Logging", "LoggingDBConnStringKey", "LogDBConnString");
		}
		#endregion

		#region Mappers
		private VRActionAudit ActionAuditMapper(IRDBDataReader reader)
		{
			return new VRActionAudit
			{
				VRActionAuditId = reader.GetLong(COL_ID),
				UserId = reader.GetNullableInt(COL_UserID),
				ModuleId = reader.GetInt(COL_ModuleID),
				EntityId = reader.GetInt(COL_EntityID),
				ActionId = reader.GetInt(COL_ActionID),
				UrlId = reader.GetInt(COL_URLID),
				ObjectId = reader.GetString(COL_ObjectID),
				ObjectName = reader.GetString(COL_ObjectName),
				ActionDescription = reader.GetString(COL_ActionDescription),
				LogTime = reader.GetDateTime(COL_LogTime)
			};

		}
		#endregion

	}
}
