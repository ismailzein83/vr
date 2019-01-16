using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class IDManagerDataManager : IIDManagerDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_IDManager";
		static string TABLE_ALIAS = "vrIDManager";
		const string COL_TypeID = "TypeID";
		const string COL_LastTakenID = "LastTakenID";
		#endregion

		#region Constructors
		static IDManagerDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_TypeID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastTakenID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "IDManager",
				Columns = columns,
				IdColumnName = COL_TypeID
			});
		}
		#endregion

		#region Public Methods
		public void ReserveIDRange(int typeId, int nbOfIds, out long startingId)
		{
			var queryContextToInsert = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContextToInsert.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_TypeID).Value(typeId);
			insertQuery.Column(COL_TypeID).Value(typeId);
			insertQuery.Column(COL_LastTakenID).Value(0);
			queryContextToInsert.ExecuteNonQuery();

			var queryContextToUpdate = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContextToUpdate.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var addExpression = updateQuery.Column(COL_LastTakenID).ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
			addExpression.Expression1().Column(COL_LastTakenID);
			addExpression.Expression2().Value(nbOfIds);
			updateQuery.Where().EqualsCondition(COL_TypeID).Value(typeId);
			queryContextToUpdate.ExecuteNonQuery();

			var queryContextToSelect = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContextToSelect.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS);
			selectQuery.SelectColumns().Column(COL_LastTakenID);
			selectQuery.Where().EqualsCondition(COL_TypeID).Value(typeId);
			startingId = queryContextToSelect.ExecuteScalar().LongValue - nbOfIds + 1;
		}

		public bool UpdateIDManager(int typeId, long lastTakenId)
		{
			try
			{
				var queryContextToUpdate = new RDBQueryContext(GetDataProvider());
				var updateQuery = queryContextToUpdate.AddUpdateQuery();
				updateQuery.FromTable(TABLE_NAME);
				updateQuery.Column(COL_LastTakenID).Value(lastTakenId);
				updateQuery.Where().EqualsCondition(COL_TypeID).Value(typeId);
				var effectedRows = queryContextToUpdate.ExecuteNonQuery();
				if (effectedRows > 0)
				{
					return true;
				}
			}
			catch (Exception ex) { }

			var queryContextToInsert = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContextToInsert.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_TypeID).Value(typeId);
			insertQuery.Column(COL_TypeID).Value(typeId);
			insertQuery.Column(COL_LastTakenID).Value(lastTakenId);
			return queryContextToInsert.ExecuteNonQuery() > 0;
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
