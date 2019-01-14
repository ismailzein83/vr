﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Data.RDB;
using Vanrise.Common;
using Vanrise.Entities;
namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class ChangedSaleZoneDataManager : IChangedSaleZoneDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_CP_SaleZone_Changed";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_EED = "EED";

        static ChangedSaleZoneDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CP_SaleZone_Changed",
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
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }
        long _processInstanceID;
        readonly string[] _columns = { COL_ID, COL_ProcessInstanceID, COL_EED };

        public void ApplyChangedZonesToDB(object preparedZones)
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

        public void WriteRecordToStream(ChangedZone record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.EntityId);
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.EED);
        }
    }
}