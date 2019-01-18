using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class RoutingProductDataManager : IRoutingProductDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "rp";
        static string TABLE_NAME = "TOneWhS_BE_RoutingProduct";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_SellingNumberPlanID = "SellingNumberPlanID";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static RoutingProductDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_SellingNumberPlanID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "RoutingProduct",
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

        #region IRoutingProductDataManager Members

        public RoutingProduct GetRoutingProduct(int routingProductId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ID).Value(routingProductId);

            return queryContext.GetItem(RoutingProductMapper);
        }

        public IEnumerable<RoutingProduct> GetRoutingProducts()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(RoutingProductMapper);
        }

        public bool Insert(RoutingProduct routingProduct, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(routingProduct.Name);

            insertQuery.Column(COL_Name).Value(routingProduct.Name);
            insertQuery.Column(COL_SellingNumberPlanID).Value(routingProduct.SellingNumberPlanId);

            if (routingProduct.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(routingProduct.Settings));

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool Update(RoutingProductToEdit routingProduct)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(routingProduct.RoutingProductId);
            notExistsCondition.EqualsCondition(COL_Name).Value(routingProduct.Name);

            updateQuery.Column(COL_Name).Value(routingProduct.Name);

            if (routingProduct.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(routingProduct.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(routingProduct.RoutingProductId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreRoutingProductsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool CheckIfRoutingProductHasRelatedSaleEntities(int routingProductId)
        {
            var routingProduct = GetRoutingProduct(routingProductId);
            return routingProduct != null;
        }
        #endregion

        #region Not Used Functions
        public BigResult<RoutingProduct> GetFilteredRoutingProducts(DataRetrievalInput<RoutingProductQuery> input)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Mappers
        Entities.RoutingProduct RoutingProductMapper(IRDBDataReader reader)
        {
            return new RoutingProduct
            {
                RoutingProductId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                SellingNumberPlanId = reader.GetInt(COL_SellingNumberPlanID),
                Settings = Serializer.Deserialize<RoutingProductSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion
    }
}
