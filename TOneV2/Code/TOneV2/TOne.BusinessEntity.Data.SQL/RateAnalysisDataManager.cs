using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class RateAnalysisDataManager : BaseTOneDataManager, IRateAnalysisDataManager
    {
       public Vanrise.Entities.BigResult<RateAnalysis> GetRateAnalysis(Vanrise.Entities.DataRetrievalInput<RateAnalysisQuery> input)
       {
           Action<string> createTempTableAction = (tempTableName) =>
           {
               ExecuteNonQuerySP("[BEntity].[sp_Rate_CreateTempByFiltered]", tempTableName, input.Query.ZoneId, input.Query.EffectedDate, input.Query.CustomerId, input.Query.SupplierId);
           };

           return RetrieveData(input, createTempTableAction, RateAnalysisMapper);
       }
       private RateAnalysis RateAnalysisMapper(IDataReader reader)
       {
           RateAnalysis rateAnalysis = new RateAnalysis();
           rateAnalysis.RateID = GetReaderValue<Int64>(reader, "RateID");
           rateAnalysis.Rate = GetReaderValue<Decimal>(reader, "Rate");
           rateAnalysis.OffPeakRate = GetReaderValue<Decimal>(reader, "OffPeakRate");
           rateAnalysis.WeekendRate = GetReaderValue<Decimal>(reader, "WeekendRate");
           rateAnalysis.Change = (Change)GetReaderValue<Int16>(reader, "Change");
           rateAnalysis.ServicesFlag = GetReaderValue<Int16>(reader, "ServicesFlag");
           rateAnalysis.BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate");
           rateAnalysis.EndEffectiveDate = GetReaderValue<DateTime>(reader, "EndEffectiveDate");
           rateAnalysis.Effective = (IsEffective)Enum.Parse(typeof(IsEffective), GetReaderValue<string>(reader, "IsEffective").ToString());
           rateAnalysis.Currency = GetReaderValue<string>(reader, "CurrencyID");
           rateAnalysis.Notes = reader["Notes"] as string;
           return rateAnalysis;
       }
    }
}
