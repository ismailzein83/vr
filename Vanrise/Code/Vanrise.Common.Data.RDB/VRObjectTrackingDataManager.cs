using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRObjectTrackingDataManager : IVRObjectTrackingDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "logging_ObjectTracking";
		static string TABLE_ALIAS = "vrLoggingObjectTracking";
		const string COL_ID = "ID";
		const string COL_UserID = "UserID";
		const string COL_LoggableEntityID = "LoggableEntityID";
		const string COL_ObjectID = "ObjectID";
		const string COL_ObjectDetails = "ObjectDetails";
		const string COL_ActionID = "ActionID";
		const string COL_ActionDescription = "ActionDescription";
		const string COL_TechnicalInformation = "TechnicalInformation";
		const string COL_LogTime = "LogTime";
		const string COL_ChangeInfo = "ChangeInfo";
		const string COL_CreatedTime = "CreatedTime";
		#endregion

		#region constructors
		static VRObjectTrackingDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LoggableEntityID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_ObjectID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_ObjectDetails, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_ActionID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_ActionDescription, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_TechnicalInformation, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_LogTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_ChangeInfo, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "logging",
				DBTableName = "ObjectTracking",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods
		public List<VRObjectTrackingMetaData> GetAll(Guid loggableEntityId, string objectId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			var selectColumns = selectQuery.SelectColumns();
			selectColumns.Column(COL_ID);
			selectColumns.Column(COL_UserID);
			selectColumns.Column(COL_ActionID);
			selectColumns.Column(COL_LogTime);
			selectColumns.Column(COL_ActionDescription);
			var trackExp1 = selectColumns.Expression(COL_ObjectDetails).CaseExpression();
			var case1 = trackExp1.AddCase();
			case1.When().NullCondition().Column(TABLE_ALIAS, COL_ObjectDetails);
			case1.Then().Value(false);
			trackExp1.Else().Value(true);

			var trackExp2 = selectColumns.Expression(COL_ChangeInfo).CaseExpression();
			var case2 = trackExp2.AddCase();
			case2.When().NullCondition().Column(TABLE_ALIAS, COL_ChangeInfo);
			case2.Then().Value(false);
			trackExp2.Else().Value(true);

			selectQuery.Where().EqualsCondition(COL_LoggableEntityID).Value(loggableEntityId);
			selectQuery.Where().EqualsCondition(COL_ObjectID).Value(objectId);

			return queryContext.GetItems(ObjectTrackingMapper);
		}
		public object GetObjectDetailById(int vrObjectTrackingId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Column(COL_ObjectDetails);
			selectQuery.Where().EqualsCondition(COL_ID).Value(vrObjectTrackingId);
			return queryContext.GetItem(ObjectDetailMapper);
		}
		public VRActionAuditChangeInfo GetVRActionAuditChangeInfoDetailById(int vrObjectTrackingId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Column(COL_ChangeInfo);
			selectQuery.Where().EqualsCondition(COL_ID).Value(vrObjectTrackingId);
			return queryContext.GetItem(VRActionAuditChangeInfoDetailMapper);
		}
		public long Insert(int userId, Guid loggableEntityId, string objectId, object obj, int actionId, string actionDescription, object technicalInformation, VRActionAuditChangeInfo vrActionAuditChangeInfo)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_UserID).Value(userId);
			insertQuery.Column(COL_LoggableEntityID).Value(loggableEntityId);
			insertQuery.Column(COL_ObjectID).Value(objectId);

			if(obj!=null)
			insertQuery.Column(COL_ObjectDetails).Value(Vanrise.Common.Serializer.Serialize(obj));

			insertQuery.Column(COL_ActionID).Value(actionId);
			insertQuery.Column(COL_ActionDescription).Value(actionDescription);
			if(technicalInformation!=null)
			insertQuery.Column(COL_TechnicalInformation).Value(Vanrise.Common.Serializer.Serialize(technicalInformation));

			if(vrActionAuditChangeInfo!=null)
			insertQuery.Column(COL_ChangeInfo).Value(Vanrise.Common.Serializer.Serialize(vrActionAuditChangeInfo));
			insertQuery.Column(COL_LogTime).DateNow();
			insertQuery.AddSelectGeneratedId();

			return  queryContext.ExecuteScalar().LongValue;

		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "LoggingDBConnStringKey", "LogDBConnString");
		}
		#endregion

		#region Mappers
		private object ObjectDetailMapper(IRDBDataReader reader)
		{
			return Vanrise.Common.Serializer.Deserialize<object>(reader.GetString(COL_ObjectDetails));
		}
		private VRActionAuditChangeInfo VRActionAuditChangeInfoDetailMapper(IRDBDataReader reader)
		{
			return Vanrise.Common.Serializer.Deserialize<VRActionAuditChangeInfo>(reader.GetString(COL_ChangeInfo));
		}
		private VRObjectTrackingMetaData ObjectTrackingMapper(IRDBDataReader reader)
		{
			return new VRObjectTrackingMetaData
			{
				VRObjectTrackingId = reader.GetLong(COL_ID),
				Time = reader.GetDateTime(COL_LogTime),
				UserId = reader.GetInt(COL_UserID),
				ActionId = reader.GetInt(COL_ActionID),
				HasDetail = reader.GetBoolean(COL_ObjectDetails),
				HasChangeInfo = reader.GetBoolean(COL_ChangeInfo),
				ActionDescription = reader.GetString(COL_ActionDescription)
			};

		}
		#endregion
	}
}
