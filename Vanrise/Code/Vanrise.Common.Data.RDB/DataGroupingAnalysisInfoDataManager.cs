using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class DataGroupingAnalysisInfoDataManager : IDataGroupingAnalysisInfoDataManager
	{
		#region Public Local Variables
		static string TABLE_NAME = "common_DataGroupingAnalysisInfo";
		static string TABLE_ALIAS = "vrDataGroupingAnalysisInfo";
		const string COL_DataAnalysisName = "DataAnalysisName";
		const string COL_DistributorServiceInstanceID = "DistributorServiceInstanceID";
		const string NbOfDataAnalysis = "NbOfDataAnalysis";
		#endregion

		#region Constructors
		static DataGroupingAnalysisInfoDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_DataAnalysisName, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_DistributorServiceInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "DataGroupingAnalysisInfo",
				Columns = columns,
				IdColumnName = COL_DataAnalysisName
			});
		}
		#endregion

		#region Public Methods
		public Dictionary<Guid, int> GetDataAnalysisCountByServiceInstanceId()
		{
			Dictionary<Guid, int> countByServiceInstanceIds = new Dictionary<Guid, int>();

			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

			var groupByContext = selectQuery.GroupBy();
			groupByContext.Select().Column(COL_DistributorServiceInstanceID);
			groupByContext.SelectAggregates().Count(NbOfDataAnalysis);
			queryContext.ExecuteReader((reader) => {
				while (reader.Read())
				{
					countByServiceInstanceIds.Add(reader.GetGuid(COL_DistributorServiceInstanceID), reader.GetInt(NbOfDataAnalysis));
				}
			});
			return countByServiceInstanceIds;

		}

		public List<string> GetDataAnalysisNames(string dataAnalysisNamePrefix)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Column(COL_DataAnalysisName);
			var whereCondition=selectQuery.Where();
			whereCondition.StartsWithCondition(COL_DataAnalysisName, dataAnalysisNamePrefix);
			return queryContext.GetItems((reader) => reader.GetString(COL_DataAnalysisName));
		}

		public void TryAssignServiceInstanceId(string dataAnalysisUniqueName, ref Guid distributorServiceInstanceId)
		{
			var queryContextToInsert = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContextToInsert.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_DataAnalysisName).Value(dataAnalysisUniqueName);
			insertQuery.Column(COL_DataAnalysisName).Value(dataAnalysisUniqueName);
			insertQuery.Column(COL_DistributorServiceInstanceID).Value(distributorServiceInstanceId);
			queryContextToInsert.ExecuteNonQuery();

			var queryContextToSelect = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContextToSelect.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Column(COL_DistributorServiceInstanceID);
			selectQuery.Where().EqualsCondition(COL_DataAnalysisName).Value(dataAnalysisUniqueName);
			distributorServiceInstanceId= queryContextToSelect.ExecuteScalar().GuidValue;
		}

		public bool TryGetAssignedServiceInstanceId(string dataAnalysisUniqueName, out Guid distributorServiceInstanceId)
		{
			var querycontext = new RDBQueryContext(GetDataProvider());
			var selectQuery = querycontext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Column(COL_DistributorServiceInstanceID);
			selectQuery.Where().EqualsCondition(COL_DataAnalysisName).Value(dataAnalysisUniqueName);
			var result=querycontext.ExecuteScalar().NullableGuidValue;
			if (result.HasValue)
			{
				distributorServiceInstanceId = result.Value;
				return true;
			}
			distributorServiceInstanceId = default(Guid);
			return false;

		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "DataGroupingDBConnStringKey", "DataGroupingDBConnString");
		}
		#endregion

		#region Mappers
		#endregion

	}
}
