using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DWDimensionDataManager : BaseSQLDataManager, IDWDimensionDataManager
    {
             

        public DWDimensionDataManager()
            : base("DWSDBConnString")
        {

        }

        public List<DWDimension> GetDimensions()
        {
            string query = string.Format("select * from {0}", this._tableName);
            return GetItemsText(query, DimensionMapper, (cmd) => { });
        }



        public void SaveDWDimensionsToDB(List<DWDimension> dwDimensions)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (DWDimension dim in dwDimensions)
            {
                WriteRecordToStream(dim, dbApplyStream);
            }

            ApplyDWDimensionsToDB(FinishDBApplyStream(dbApplyStream));
        }

        public void ApplyDWDimensionsToDB(object preparedDWDimensions)
        {
            InsertBulkToTable(preparedDWDimensions as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = this._tableName,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(DWDimension record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}",
                                     record.Id
                                   , record.Description
                                    );

        }


        #region Private Methods

        private DWDimension DimensionMapper(IDataReader reader)
        {
            var dimension = new DWDimension();
            dimension.Id = (int)reader[0];
            dimension.Description = reader[1] as string;
            return dimension;
        }

        #endregion

        string _tableName;

        public string TableName
        {
            set { _tableName = value; }
        }
    }
}
