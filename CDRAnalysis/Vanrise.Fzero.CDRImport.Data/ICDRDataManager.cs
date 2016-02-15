using System;
using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;



namespace Vanrise.Fzero.CDRImport.Data
{
    public interface ICDRDataManager : IDataManager, IBulkApplyDataManager<CDR>
    {

        void ApplyCDRsToDB(object preparedCDRs);

        void SaveCDRsToDB(List<CDR> cdrs);

        void LoadCDR(List<string> numberPrefix, DateTime from, DateTime to, int? batchSize, Action<CDR> onCDRReady);

        BigResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRQuery> input);
    }
}
