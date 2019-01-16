using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistCustomerChangeNewDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistCustomerChange_New";
        static string TABLE_ALIAS = "spcucnew";
        const string COL_BatchID = "BatchID";
        const string COL_PricelistID = "PricelistID";
        const string COL_CountryID = "CountryID";
        const string COL_CustomerID = "CustomerID";


        static SalePricelistCustomerChangeNewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_BatchID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_PricelistID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePricelistCustomerChange_New",
                Columns = columns
            });
        }

        #endregion

        #region Public Methods
        public void DeleteRecords(RDBDeleteQuery deleteQuery, long processInstanceId)
        {
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_BatchID).Value(processInstanceId);
        }

        public void BuildSelectQuery(RDBSelectQuery selectQuery, long processInstanceId)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_BatchID, COL_PricelistID, COL_CountryID, COL_CustomerID);
            selectQuery.Where().EqualsCondition(COL_BatchID).Value(processInstanceId);
        }
        #endregion
    }
}
