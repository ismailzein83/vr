using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Data.RDB
{
    public class DaysToReprocessDataManager : IDaysToReprocessDataManager
    {
        static string TABLE_NAME = "TOneWhS_Deal_DaysToReprocess";
        static string TABLE_ALIAS = "dtr";
        const string COL_ID = "ID";
        const string COL_Date = "Date";
        const string COL_IsSale = "IsSale";
        const string COL_CarrierAccountId = "CarrierAccountId";

        static DaysToReprocessDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Date, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_IsSale, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CarrierAccountId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Deal",
                DBTableName = "DaysToReprocess",
                Columns = columns,
                IdColumnName = COL_ID
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Deal", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        public IEnumerable<DayToReprocess> GetAllDaysToReprocess()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(DaysToReprocessMapper);
        }

        public bool Insert(DateTime date, bool isSale, int carrierAccountId, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Date).Value(date);
            notExistsCondition.EqualsCondition(COL_IsSale).Value(isSale);
            notExistsCondition.EqualsCondition(COL_CarrierAccountId).Value(carrierAccountId);

            insertQuery.Column(COL_Date).Value(date);
            insertQuery.Column(COL_IsSale).Value(isSale);
            insertQuery.Column(COL_CarrierAccountId).Value(carrierAccountId);

            insertedId = queryContext.ExecuteScalar().IntValue;
            if (insertedId > 0)
                return true;
            return false;
        }

        public void DeleteDaysToReprocess()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            queryContext.ExecuteNonQuery();
        }

        public void DeleteDaysToReprocessByDate(DateTime date)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var where = deleteQuery.Where();
            where.EqualsCondition(COL_Date).Value(date);

            queryContext.ExecuteNonQuery();
        }

        private DayToReprocess DaysToReprocessMapper(IRDBDataReader reader)
        {
            return new DayToReprocess
            {
                DayToReprocessId = reader.GetLong(COL_ID),
                Date = reader.GetDateTime(COL_Date),
                IsSale = reader.GetBoolean(COL_IsSale),
                CarrierAccountId = reader.GetInt(COL_CarrierAccountId)
            };
        }
    }
}
