using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRRuleDataManager : IVRRuleDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRRule";
		static string TABLE_ALIAS = "vrRule";
		const string COL_ID = "ID";
		const string COL_RuleDefinitionId = "RuleDefinitionId";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_IsDeleted = "IsDeleted";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRRuleDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_RuleDefinitionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRRule",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime,
				CachePartitionColumnName = COL_RuleDefinitionId
			});
		}
		#endregion

		#region Public Methods
		public bool AreRulesUpdated(Guid vrRuleDefinitionId, ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, vrRuleDefinitionId, ref updateHandle);
		}
		public bool DeleteVRRules(List<long> vrRuleIds)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_IsDeleted).Value(1);
			updateQuery.Where().ListCondition(COL_ID, RDBListConditionOperator.IN, vrRuleIds);
			return queryContext.ExecuteNonQuery() > 0;
		}
		public IEnumerable<VRRule> GetVRRules(Guid vrRuleDefinitionId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Where().EqualsCondition(COL_RuleDefinitionId).Value(vrRuleDefinitionId);
			var condition = selectQuery.Where().ConditionIfColumnNotNull(COL_IsDeleted, RDBConditionGroupOperator.OR);
			condition.EqualsCondition(COL_IsDeleted).Value(false);
			return queryContext.GetItems(VRRuleMapper);
		}
		public bool Insert(VRRule vrRule, out long vrRuleId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_RuleDefinitionId).Value(vrRule.VRRuleDefinitionId);
			if (vrRule.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrRule.Settings));

			insertQuery.AddSelectGeneratedId();
			var insertedId = queryContext.ExecuteScalar().NullableLongValue;

			if (insertedId.HasValue)
			{
				vrRuleId = insertedId.Value;
				return true;
			}
			vrRuleId = -1;
			return false;
		}
		public bool Update(VRRule vrRule)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_RuleDefinitionId).Value(vrRule.VRRuleDefinitionId);
			if (vrRule.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrRule.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrRule.VRRuleId);

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
		private VRRule VRRuleMapper(IRDBDataReader reader)
		{
			return new VRRule
			{
				VRRuleId = reader.GetLong(COL_ID),
				VRRuleDefinitionId = reader.GetGuid(COL_RuleDefinitionId),
				Settings = Vanrise.Common.Serializer.Deserialize<VRRuleSettings>(reader.GetString(COL_Settings))
			};
		}
		#endregion
	}
}
