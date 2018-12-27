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
		#region Local Variables
		static string TABLE_NAME = "common_RateType";
		static string TABLE_ALIAS = "vrRateType";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
        const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static RateTypeDataManager()
		{
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "common",
                DBTableName = "RateType",
                Columns = columns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
		#endregion

		#region Public Methods
		public bool AreRateTypesUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

		public List<RateType> GetRateTypes()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
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
            var insertedID = queryContext.ExecuteScalar().NullableIntValue;
            if (insertedID.HasValue)
            {
                insertedId = insertedID.Value;
                return true;
            }
            insertedId = -1;
            return false;
        }

		public bool Update(RateType rateType)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(rateType.RateTypeId);
			ifNotExist.EqualsCondition(COL_Name).Value(rateType.Name);
			updateQuery.Column(COL_Name).Value(rateType.Name);
			updateQuery.Where().EqualsCondition(COL_ID).Value(rateType.RateTypeId);
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
