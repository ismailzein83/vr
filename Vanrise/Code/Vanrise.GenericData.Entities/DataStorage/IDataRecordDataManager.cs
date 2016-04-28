﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IDataRecordDataManager : Vanrise.Data.IBulkApplyDataManager<Object>
    {
        void ApplyStreamToDB(object stream);

        List<DataRecord> GetFilteredDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input);
    }

    public interface ISummaryRecordDataManager
    {
        void InsertSummaryRecords(IEnumerable<dynamic> records);

        void UpdateSummaryRecords(IEnumerable<dynamic> records);

        IEnumerable<dynamic> GetExistingSummaryRecords(DateTime batchStart);
    }
}
