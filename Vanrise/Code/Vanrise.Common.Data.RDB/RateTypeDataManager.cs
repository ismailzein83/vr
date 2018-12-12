using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class RateTypeDataManager : IRateTypeDataManager
	{
		static string TABLE_NAME = "common_RateType";
		static string TABLE_ALIAS = "rateType";
		const string COL_ID = "ID";
		const string COL_Name = "Name";

		static RateTypeDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "RateType",
				Columns = columns,
				IdColumnName = COL_ID
			});
		}

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_RateType", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Public Methods
		public bool AreRateTypesUpdated(ref object updateHandle)
		{
			throw new NotImplementedException();
		}

		public List<RateType> GetRateTypes()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().Columns(COL_ID, COL_Name);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(RateTypeMapper);
		}

		public bool Insert(RateType rateType, out int insertedId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(rateType.Name);
			insertQuery.AddSelectGeneratedId();
			insertQuery.Column(COL_Name).Value(rateType.Name);
			insertedId = queryContext.ExecuteScalar().IntWithNullHandlingValue;
			if (insertedId == 0)
				insertedId = -1;
			return (insertedId != -1);
		}

		public bool Update(RateType rateType)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.NotEqualsCondition(COL_ID).Value(rateType.RateTypeId);
			ifNotExist.EqualsCondition(COL_Name).Value(rateType.Name);
			updateQuery.Column(COL_Name).Value(rateType.Name);
			updateQuery.Where().EqualsCondition(COL_ID).Value(rateType.RateTypeId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Mappers
		RateType RateTypeMapper(IRDBDataReader reader)
		{
			return new RateType
			{
				Name = reader.GetString(COL_Name),
				RateTypeId = reader.GetInt(COL_ID)
			};
		}
		#endregion
	}

}
