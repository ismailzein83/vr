using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.SMSBusinessEntity.Data.RDB
{
    public class CustomerSMSRateDataManager: ICustomerSMSRateDataManager
    {
        static string TABLE_NAME = "TOneWhS_SMSBE_CustomerSMSRate";
        static string TABLE_ALIAS = "CustomerSMSRate";
        const string COL_ID = "ID";
        const string COL_PriceListID = "PriceListID";
        const string COL_MobileNetworkID = "MobileNetworkID";
        const string COL_Rate = "Rate";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_LastModified = "LastModified";
        const string COL_CreatedTime = "CreatedTime";


        static CustomerSMSRateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PriceListID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_MobileNetworkID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Rate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 18, Precision = 0 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModified, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_SMSBE",
                DBTableName = "CustomerSMSRate",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_SMSBuisenessEntity", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }


        public List<CustomerSMSRate> GetCustomerSMSRates(int customerID)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_ID);
            selectQuery.SelectColumns().Column(COL_PriceListID);
            selectQuery.SelectColumns().Column(COL_MobileNetworkID);
            selectQuery.SelectColumns().Column(COL_Rate);
            selectQuery.SelectColumns().Column(COL_BED);
            selectQuery.SelectColumns().Column(COL_EED);
            new CustomerSMSPriceListDataManager().Join(selectQuery, TABLE_ALIAS, COL_PriceListID, customerID);

            return queryContext.GetItems(CustomerSMSRateMapper);
        }

        private CustomerSMSRate CustomerSMSRateMapper(IRDBDataReader dataReader)
        {
            return new CustomerSMSRate()
            {
                ID = dataReader.GetInt(COL_ID),
                PriceListID = dataReader.GetInt(COL_PriceListID),
                MobileNetworkID = dataReader.GetInt(COL_MobileNetworkID),
                Rate = dataReader.GetNullableDecimal(COL_Rate),
                BED = dataReader.GetDateTime(COL_BED),
                EED = dataReader.GetNullableDateTime(COL_EED)
            };
        }
    }
}
