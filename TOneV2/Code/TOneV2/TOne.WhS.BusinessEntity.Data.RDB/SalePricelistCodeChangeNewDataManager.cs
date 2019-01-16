using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistCodeChangeNewDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistCodeChange_New";
        static string TABLE_ALIAS = "spccnew";
        const string COL_Code = "Code";
        const string COL_RecentZoneName = "RecentZoneName";
        const string COL_ZoneName = "ZoneName";
        const string COL_ZoneID = "ZoneID";
        const string COL_Change = "Change";
        const string COL_BatchID = "BatchID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_CountryID = "CountryID";


        static SalePricelistCodeChangeNewDataManager()
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
                DBTableName = "SalePricelistCodeChange_New",
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

        public void BuildSelectQuery(RDBSelectQuery selectQuery, long processInstanceID)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_Code, COL_RecentZoneName, COL_ZoneName, COL_ZoneID, COL_Change, COL_BatchID, COL_BED, COL_EED, COL_CountryID);
            selectQuery.Where().EqualsCondition(COL_BatchID).Value(processInstanceID);
        }
        #endregion
    }
}
