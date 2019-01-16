using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRExclusiveSessionDataManager : IVRExclusiveSessionDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRExclusiveSession";
		static string TABLE_ALIAS = "vrExclusiveSession";
		const string COL_ID = "ID";
		const string COL_SessionTypeId = "SessionTypeId";
		const string COL_TargetId = "TargetId";
		const string COL_TakenByUserId = "TakenByUserId";
		const string COL_LastTakenUpdateTime = "LastTakenUpdateTime";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_TakenTime = "TakenTime";
		#endregion

		#region Contructors
		static VRExclusiveSessionDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_SessionTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_TargetId, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 400 });
			columns.Add(COL_TakenByUserId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastTakenUpdateTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_TakenTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRExclusiveSession",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods
		public void ForceReleaseAllSessions()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_TakenByUserId).Null();
			updateQuery.Column(COL_LastTakenUpdateTime).Null();
			queryContext.ExecuteNonQuery();
		}

		public void ForceReleaseSession(int vrExclusiveSessionId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_TakenByUserId).Null();
			updateQuery.Column(COL_LastTakenUpdateTime).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrExclusiveSessionId);
			queryContext.ExecuteNonQuery();
		}

		public List<VRExclusiveSession> GetAllVRExclusiveSessions(int timeOutInSeconds, List<Guid> sessionTypeIds)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);

			selectQuery.SelectColumns().Columns(COL_ID, COL_SessionTypeId, COL_TargetId, COL_TakenByUserId, COL_LastTakenUpdateTime, COL_CreatedTime, COL_TakenTime);
			var whereConditions = selectQuery.Where(RDBConditionGroupOperator.AND);
			whereConditions.NotNullCondition(COL_TakenByUserId);
			whereConditions.NotNullCondition(COL_LastTakenUpdateTime);
			var childConditionGroup = whereConditions.ChildConditionGroup();
			var compareConditionGroupContext = childConditionGroup.CompareCondition(RDBCompareConditionOperator.L);
			var dateTimeDiffContext = compareConditionGroupContext.Expression1().DateTimeDiff(RDBDateTimeDiffInterval.Seconds);
			dateTimeDiffContext.DateTimeExpression1().Column(TABLE_ALIAS, COL_LastTakenUpdateTime);
			dateTimeDiffContext.DateTimeExpression2().DateNow();
			compareConditionGroupContext.Expression2().Value(timeOutInSeconds);
			if (sessionTypeIds != null && sessionTypeIds.Count > 0)
				whereConditions.ListCondition(COL_SessionTypeId, RDBListConditionOperator.IN, sessionTypeIds);

			return queryContext.GetItems(VRExclusiveSessionMapper);
		}

		public void InsertIfNotExists(Guid sessionTypeId, string targetId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);

			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(TABLE_ALIAS, COL_SessionTypeId).Value(sessionTypeId);
			ifNotExist.EqualsCondition(TABLE_ALIAS, COL_TargetId).Value(targetId);

			insertQuery.Column(COL_SessionTypeId).Value(sessionTypeId);
			insertQuery.Column(COL_TargetId).Value(targetId);

			queryContext.ExecuteNonQuery();
		}

		public void ReleaseSession(Guid sessionTypeId, string targetId, int userId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_TakenByUserId).Null();
			updateQuery.Column(COL_LastTakenUpdateTime).Null();
			updateQuery.Where().EqualsCondition(COL_SessionTypeId).Value(sessionTypeId);
			updateQuery.Where().EqualsCondition(COL_TargetId).Value(targetId);
			updateQuery.Where().EqualsCondition(COL_TakenByUserId).Value(userId);
			queryContext.ExecuteNonQuery();
		}

		public void TryKeepSession(Guid sessionTypeId, string targetId, int userId, int timeoutInSeconds, out int? takenByUserId)
		{
			var queryContextToUpdate = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContextToUpdate.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_LastTakenUpdateTime).DateNow();
			updateQuery.Where().EqualsCondition(COL_SessionTypeId).Value(sessionTypeId);
			updateQuery.Where().EqualsCondition(COL_TargetId).Value(targetId);
			updateQuery.Where().EqualsCondition(COL_TakenByUserId).Value(userId);
			queryContextToUpdate.ExecuteNonQuery();

			var queryContextToSelect = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContextToSelect.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Column(COL_TakenByUserId);
			selectQuery.Where().EqualsCondition(COL_SessionTypeId).Value(sessionTypeId);
			selectQuery.Where().EqualsCondition(COL_TargetId).Value(targetId);
			takenByUserId = queryContextToSelect.ExecuteScalar().NullableIntValue;
		}

		public void TryTakeSession(Guid sessionTypeId, string targetId, int userId, int timeoutInSeconds, out int takenByUserId)
		{
			var queryContextToUpdate = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContextToUpdate.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);

			updateQuery.Column(COL_TakenByUserId).Value(userId);
			updateQuery.Column(COL_LastTakenUpdateTime).DateNow();
			updateQuery.Column(COL_TakenTime).DateNow();
			updateQuery.Where().EqualsCondition(COL_SessionTypeId).Value(sessionTypeId);
			updateQuery.Where().EqualsCondition(COL_TargetId).Value(targetId);
			var childConditionGroup = updateQuery.Where().ChildConditionGroup(RDBConditionGroupOperator.OR);
			childConditionGroup.ConditionIfColumnNotNull(COL_TakenByUserId).EqualsCondition(COL_TakenByUserId).Value(userId);
			var compareConditionGroupContext = childConditionGroup.CompareCondition(RDBCompareConditionOperator.G);
			var dateTimeContextCondition = compareConditionGroupContext.Expression1().DateTimeDiff(RDBDateTimeDiffInterval.Seconds);
			dateTimeContextCondition.DateTimeExpression1().Column(COL_LastTakenUpdateTime);
			dateTimeContextCondition.DateTimeExpression2().DateNow();
			compareConditionGroupContext.Expression2().Value(timeoutInSeconds);
			queryContextToUpdate.ExecuteNonQuery();

			var queryContextToSelect = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContextToSelect.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Column(COL_TakenByUserId);
			selectQuery.Where().EqualsCondition(TABLE_ALIAS, COL_SessionTypeId).Value(sessionTypeId);
			selectQuery.Where().EqualsCondition(TABLE_ALIAS, COL_TargetId).Value(targetId);
			takenByUserId = queryContextToSelect.ExecuteScalar().IntValue;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private VRExclusiveSession VRExclusiveSessionMapper(IRDBDataReader reader)
		{
			return new VRExclusiveSession
			{
				VRExclusiveSessionID = reader.GetInt(COL_ID),
				SessionTypeId =reader.GetGuid(COL_SessionTypeId),
				TargetId = reader.GetString(COL_TargetId),
				TakenByUserId = reader.GetInt(COL_TakenByUserId),
				LastTakenUpdateTime = reader.GetDateTime(COL_LastTakenUpdateTime),
				CreatedTime = reader.GetDateTime(COL_CreatedTime),
				TakenTime =reader.GetDateTime(COL_TakenTime)
			};
		}
		#endregion

	}
}
