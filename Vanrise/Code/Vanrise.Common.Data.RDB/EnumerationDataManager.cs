using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class EnumerationDataManager : IEnumerationDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_Enumerations";
		static string TABLE_ALIAS = "vrEnumerations";
		const string COL_ID = "ID";
		const string COL_NameSpace = "NameSpace";
		const string COL_Name = "Name";
		const string COL_Description = "Description";
		readonly string[] _columns = { COL_ID, COL_NameSpace, COL_Name,COL_Description};

		#endregion

		#region Constructors
		static EnumerationDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_NameSpace, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
			columns.Add(COL_Description, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "Enumerations",
				Columns = columns,
				IdColumnName = COL_ID
			});
		}
		#endregion

		#region Public Methods
		public void ClearEnumerations()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var deleteQuery = queryContext.AddDeleteQuery();
			deleteQuery.FromTable(TABLE_NAME);
			queryContext.ExecuteNonQuery();
		}

		public void SaveEnumerationsToDb(IEnumerable<Enumeration> enumerations)
		{
			Object dbApplyStream = InitialiazeStreamForDBApply();
			foreach (Enumeration enumeration in enumerations)
				WriteRecordToStream(enumeration, dbApplyStream);
			Object preparedEnumerations = FinishDBApplyStream(dbApplyStream);
			ApplyEnumerationsToDB(preparedEnumerations);
		}

		public void WriteRecordToStream(Enumeration record, object dbApplyStream)
		{
			RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
			var recordContext = bulkInsertContext.WriteRecord();
			recordContext.Value(record.ID);
			recordContext.Value(record.NameSpace);
			recordContext.Value(record.Name);
			recordContext.Value(record.Description);
		}

		public object InitialiazeStreamForDBApply()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var streamForBulkInsert = queryContext.StartBulkInsert();
			streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
			return streamForBulkInsert;
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
			bulkInsertContext.CloseStream();
			return bulkInsertContext;
		}

		public void ApplyEnumerationsToDB(object preparedObject)
		{
			preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("preparedObject").Apply();
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
