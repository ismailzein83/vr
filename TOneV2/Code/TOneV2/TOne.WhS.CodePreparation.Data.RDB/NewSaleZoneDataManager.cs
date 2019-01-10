using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class NewSaleZoneDataManager : INewSaleZoneDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_CP_SaleZone_New";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_CountryID = "CountryID";
        const string COL_Name = "Name";
        const string COL_SellingNumberPlanID = "SellingNumberPlanID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";


        static NewSaleZoneDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_SellingNumberPlanID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CP_SaleZone_New",
                Columns = columns
            });
        }


        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("WhS_CodePrep", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region INewSaleZoneDataManager

        public int SellingNumberPlanId
        {
            set
            {
                _sellingNumberPlanId = value;
            }
        }

        int _sellingNumberPlanId;
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }
        long _processInstanceID;
        readonly string[] _columns = { COL_ID, COL_ProcessInstanceID, COL_CountryID, COL_Name, COL_SellingNumberPlanID, COL_BED, COL_EED };

        public void ApplyNewZonesToDB(object preparedZones)
        {
            preparedZones.CastWithValidate<RDBBulkInsertQueryContext>("preparedZones").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(AddedZone record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.ZoneId);
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.CountryId);
            recordContext.Value(record.Name);
            recordContext.Value(_sellingNumberPlanId);
            recordContext.Value(record.BED);
            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Null();
        }
        #endregion
    }
}
