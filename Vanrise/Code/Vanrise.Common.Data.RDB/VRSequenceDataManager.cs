using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRSequenceDataManager : IVRSequenceDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRSequence";
		static string TABLE_ALIAS = "vrSequence";
		const string COL_SequenceGroup = "SequenceGroup";
		const string COL_SequenceDefinitionID = "SequenceDefinitionID";
		const string COL_SequenceKey = "SequenceKey";
		const string COL_InitialValue = "InitialValue";
		const string COL_LastValue = "LastValue";
		const string COL_CreatedTime = "CreatedTime";
		#endregion

		#region Constructors
		static VRSequenceDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_SequenceGroup, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_SequenceDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_SequenceKey, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_InitialValue, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_LastValue, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRSequence",
				Columns = columns,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods
		public long GetNextSequenceValue(string sequenceGroup, Guid sequenceDefinitionId, string sequenceKey, long initialValue, long? reserveNumber)
		{
			long? nextSequence = null;
			long effectiveInitialValue;
			var reserveNumberValue = reserveNumber.HasValue ? reserveNumber.Value : 1;

			var queryContextToSelect = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContextToSelect.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Where().EqualsCondition(COL_SequenceDefinitionID).Value(sequenceDefinitionId);
			selectQuery.Where().EqualsCondition(COL_SequenceGroup).Value(sequenceGroup);
			var effectedRows = queryContextToSelect.GetItems((reader) => { return reader; }).Count();
			if (effectedRows <= 0)
			{
				effectiveInitialValue = initialValue;
			}
			else
			{
				effectiveInitialValue = reserveNumberValue;
			}


			var queryContextToInsert = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContextToInsert.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_SequenceDefinitionID).Value(sequenceDefinitionId);
			ifNotExist.EqualsCondition(COL_SequenceKey).Value(sequenceKey);
			ifNotExist.EqualsCondition(COL_SequenceGroup).Value(sequenceGroup);

			insertQuery.Column(COL_SequenceGroup).Value(sequenceGroup);
			insertQuery.Column(COL_SequenceDefinitionID).Value(sequenceDefinitionId);
			insertQuery.Column(COL_SequenceKey).Value(sequenceKey);
			insertQuery.Column(COL_InitialValue).Value(effectiveInitialValue);
			insertQuery.Column(COL_LastValue).Value(effectiveInitialValue);

			effectedRows = queryContextToInsert.ExecuteNonQuery();
			if (effectedRows > 0)
			{
				nextSequence = effectiveInitialValue;
			}

			if (!nextSequence.HasValue)
			{
				var queryContextToUpdate = new RDBQueryContext(GetDataProvider());
				var updateQuery = queryContextToUpdate.AddUpdateQuery();
				updateQuery.FromTable(TABLE_NAME);

				var addExpression = updateQuery.Column(COL_LastValue).ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
				addExpression.Expression1().Column(COL_LastValue);
				addExpression.Expression2().Value(reserveNumberValue);
				updateQuery.Where().EqualsCondition(COL_SequenceDefinitionID).Value(sequenceDefinitionId);
				updateQuery.Where().EqualsCondition(COL_SequenceKey).Value(sequenceKey);
				updateQuery.Where().EqualsCondition(COL_SequenceGroup).Value(sequenceGroup);

				queryContextToUpdate.ExecuteNonQuery();

				queryContextToSelect = new RDBQueryContext(GetDataProvider());
				selectQuery = queryContextToSelect.AddSelectQuery();
				selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
				selectQuery.SelectColumns().Column(COL_LastValue);
				selectQuery.Where().EqualsCondition(COL_SequenceDefinitionID).Value(sequenceDefinitionId);
				selectQuery.Where().EqualsCondition(COL_SequenceKey).Value(sequenceKey);
				selectQuery.Where().EqualsCondition(COL_SequenceGroup).Value(sequenceGroup);
				return queryContextToSelect.ExecuteScalar().LongValue;
			}
			return nextSequence.Value;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		#endregion
	}
}
