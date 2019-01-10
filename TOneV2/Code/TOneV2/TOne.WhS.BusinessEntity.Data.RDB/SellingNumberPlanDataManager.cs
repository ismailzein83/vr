using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SellingNumberPlanDataManager : ISellingNumberPlanDataManager
    {
        static string TABLE_ALIAS = "snp";
        static string TABLE_NAME = "TOneWhS_BE_SellingNumberPlan";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static SellingNumberPlanDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Name, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 255}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SellingNumberPlan",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #region ISellingNumberPlanDataManager Members
        public List<SellingNumberPlan> GetSellingNumberPlans()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SellingNumberPlanMapper);
        }

        public bool AreSellingNumberPlansUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool Update(SellingNumberPlanToEdit sellingNumberPlan)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(sellingNumberPlan.SellingNumberPlanId);
            notExistsCondition.EqualsCondition(COL_Name).Value(sellingNumberPlan.Name);

            updateQuery.Column(COL_Name).Value(sellingNumberPlan.Name);

            if (sellingNumberPlan.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(sellingNumberPlan.LastModifiedBy.Value);
            
            updateQuery.Column(COL_LastModifiedTime).DateNow();

            updateQuery.Where().EqualsCondition(COL_ID).Value(sellingNumberPlan.SellingNumberPlanId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Insert(SellingNumberPlan sellingNumberPlan, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.OR);
            notExistsCondition.EqualsCondition(COL_Name).Value(sellingNumberPlan.Name);

            insertQuery.Column(COL_Name).Value(sellingNumberPlan.Name);

            if (sellingNumberPlan.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(sellingNumberPlan.CreatedBy.Value);

            if (sellingNumberPlan.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(sellingNumberPlan.LastModifiedBy.Value);

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }
        #endregion

        #region Mappers
        SellingNumberPlan SellingNumberPlanMapper(IRDBDataReader reader)
        {
            SellingNumberPlan sellingNumberPlan = new SellingNumberPlan
            {
                SellingNumberPlanId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                CreatedTime = reader.GetNullableDateTime(COL_CreatedTime),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
            };
            return sellingNumberPlan;
        }
        #endregion
    }
}
