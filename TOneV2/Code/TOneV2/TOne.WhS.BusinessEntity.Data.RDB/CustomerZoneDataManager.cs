using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class CustomerZoneDataManager : ICustomerZoneDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "cz";
        static string TABLE_NAME = "TOneWhS_BE_CustomerZone";
        const string COL_ID = "ID";
        const string COL_CustomerID = "CustomerID";
        const string COL_Details = "Details";
        const string COL_BED = "BED";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static CustomerZoneDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CustomerID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Details, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CustomerZone",
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

        #region ICustomerZoneDataManager Members

        public List<CustomerZones> GetAllCustomerZones()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(CustomerZonesMapper);
        }

        public bool AddCustomerZones(CustomerZones customerZones, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            insertQuery.Column(COL_CustomerID).Value(customerZones.CustomerId);
            insertQuery.Column(COL_BED).Value(customerZones.StartEffectiveTime);

            if (customerZones.Countries != null)
                insertQuery.Column(COL_Details).Value(Serializer.Serialize(customerZones.Countries));

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool AreAllCustomerZonesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
        #endregion

        #region Mappers

        private CustomerZones CustomerZonesMapper(IRDBDataReader reader)
        {
            return new CustomerZones
            {
                CustomerZonesId = reader.GetInt(COL_ID),
                CustomerId = reader.GetInt(COL_CustomerID),
                Countries = Serializer.Deserialize<List<CustomerCountry>>(reader.GetString(COL_Details)),
                StartEffectiveTime = reader.GetDateTime(COL_BED)
            };
        }

        #endregion
    }
}
