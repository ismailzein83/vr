using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SellingProductDataManager : ISellingProductDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sp";
        static string TABLE_NAME = "TOneWhS_BE_SellingProduct";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_DefaultRoutingProductID = "DefaultRoutingProductID";
        const string COL_SellingNumberPlanID = "SellingNumberPlanID";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SellingProductDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_Name, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 255});
            columns.Add(COL_DefaultRoutingProductID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_SellingNumberPlanID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_Settings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar});
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SellingProduct",
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

        #endregion

        #region ISellingProductDataManager Members
        public List<SellingProduct> GetSellingProducts()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SellingProductMapper);
        }

        public bool Insert(SellingProduct sellingProduct, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(sellingProduct.Name);

            insertQuery.Column(COL_Name).Value(sellingProduct.Name);
            insertQuery.Column(COL_SellingNumberPlanID).Value(sellingProduct.SellingNumberPlanId);

            if (sellingProduct.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(sellingProduct.Settings));

            if (sellingProduct.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(sellingProduct.CreatedBy.Value);

            if (sellingProduct.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(sellingProduct.LastModifiedBy.Value);

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool Update(SellingProductToEdit sellingProduct)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.OR);
            notExistsCondition.EqualsCondition(COL_Name).Value(sellingProduct.Name);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(sellingProduct.SellingProductId);

            updateQuery.Column(COL_Name).Value(sellingProduct.Name);
            
            if (sellingProduct.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(sellingProduct.Settings));
            
            if (sellingProduct.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(sellingProduct.LastModifiedBy.Value);
            
            updateQuery.Column(COL_LastModifiedTime).DateNow();

            updateQuery.Where().EqualsCondition(COL_ID).Value(sellingProduct.SellingProductId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreSellingProductsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
        #endregion

        #region Mappers
        SellingProduct SellingProductMapper(IRDBDataReader reader)
        {
            SellingProduct sellingProductDetail = new SellingProduct
            {
                SellingProductId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                SellingNumberPlanId = reader.GetInt(COL_SellingNumberPlanID),
                Settings = Serializer.Deserialize<SellingProductSettings>(reader.GetString(COL_Settings)),
                CreatedTime = reader.GetNullableDateTime(COL_CreatedTime),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
            };
            return sellingProductDetail;
        }
        #endregion
    }
}
