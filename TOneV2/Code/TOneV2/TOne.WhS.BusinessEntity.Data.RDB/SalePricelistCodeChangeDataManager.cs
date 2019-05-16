﻿using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistCodeChangeDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "spcc";
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistCodeChange";
        const string COL_Code = "Code";
        const string COL_RecentZoneName = "RecentZoneName";
        const string COL_ZoneName = "ZoneName";
        const string COL_ZoneID = "ZoneID";
        const string COL_Change = "Change";
        const string COL_BatchID = "BatchID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_CountryID = "CountryID";

        static SalePricelistCodeChangeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 150 });
            columns.Add(COL_RecentZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 150 });
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 150 });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Change, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BatchID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePricelistCodeChange",
                Columns = columns
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region Public Methods

        public List<SalePricelistCodeChange> GetSalePricelistCodeChanges(int pricelistId, List<int> countryIds)
        {
            var salePricelistCustomerChangeDataManager = new SalePricelistCustomerChangeDataManager();
            var salePricelistCustomerChangeTableAlias = "spcustc";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectContext = selectQuery.SelectColumns();
            var join = selectQuery.Join();
            salePricelistCustomerChangeDataManager.JoinCustomerChange(join, salePricelistCustomerChangeTableAlias, TABLE_ALIAS, SalePricelistCustomerChangeDataManager.COL_CountryID, SalePricelistCustomerChangeDataManager.COL_BatchID);
            
            selectContext.AllTableColumns(TABLE_ALIAS);
            selectContext.Column(salePricelistCustomerChangeTableAlias, SalePricelistCustomerChangeDataManager.COL_PricelistID, SalePricelistCustomerChangeDataManager.COL_PricelistID);
            
            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(salePricelistCustomerChangeTableAlias, SalePricelistCustomerChangeDataManager.COL_PricelistID).Value(pricelistId);

            if (countryIds != null && countryIds.Count > 0)
                whereQuery.ListCondition(salePricelistCustomerChangeTableAlias, SalePricelistCustomerChangeDataManager.COL_CountryID, RDBListConditionOperator.IN, countryIds);

            return queryContext.GetItems(SalePricelistCodeChangeMapper);
        }
        public void BuildInsertQuery(RDBInsertQuery insertQuery)
        {
            insertQuery.IntoTable(TABLE_NAME);
        }

        public void SetUpdateContext(RDBQueryContext queryContext, string zoneName, long zoneId)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_ZoneName).Value(zoneName);
            updateQuery.Where().EqualsCondition(COL_ZoneID).Value(zoneId);
        }
        #endregion

        #region Mappers

        SalePricelistCodeChange SalePricelistCodeChangeMapper(IRDBDataReader reader)
        {
            return new SalePricelistCodeChange
            {
                PricelistId = reader.GetInt(SalePricelistCustomerChangeDataManager.COL_PricelistID),
                Code = reader.GetString(COL_Code),
                CountryId = reader.GetInt(COL_CountryID),
                RecentZoneName = reader.GetString(COL_RecentZoneName),
                ZoneName = reader.GetString(COL_ZoneName),
                ChangeType = (CodeChange)reader.GetInt(COL_Change),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                ZoneId = reader.GetNullableLong(COL_ZoneID)
            };
        }
        #endregion
    }
}
