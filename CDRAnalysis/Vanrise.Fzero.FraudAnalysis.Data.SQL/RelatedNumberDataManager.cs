using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class RelatedNumberDataManager : BaseSQLDataManager, IRelatedNumberDataManager
    {

        #region ctor
        public RelatedNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }
        #endregion

        #region Public Methods
        public void SavetoDB(Dictionary<string, string> records)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (KeyValuePair<string, string> record in records)
            {
                stream.WriteRecord("{0}*{1}",
                                record.Key,
                                record.Value
                                );
            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[RelatedNumber]",
                    Stream = stream,
                    TabLock = true,
                    KeepIdentity = false,
                    FieldSeparator = '*'
                });
        }

        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            return GetItemsSP("FraudAnalysis.sp_RelatedNumber_GetByAccountNumber", RelatedNumberMapper, accountNumber);
        }

        public void LoadRelatedNumberByNumberPrefix(string numberPrefix, Action<RelatedNumber> onBatchReady)
        {

            ExecuteReaderSP("[FraudAnalysis].[sp_RelatedNumber_GetByNumberPrefix]", (reader) =>
            {
                while (reader.Read())
                {
                    onBatchReady(RelatedNumberMapper(reader));
                }
            }, numberPrefix);
        }

        #endregion

        # region Mappers
        private RelatedNumber RelatedNumberMapper(IDataReader reader)
        {
            RelatedNumber relatedNumber = new RelatedNumber();
            relatedNumber.AccountNumber = reader["AccountNumber"] as string;
            relatedNumber.RelatedAccountNumber = reader["RelatedAccountNumber"] as string;
            return relatedNumber;
        }
        # endregion

    }
}
