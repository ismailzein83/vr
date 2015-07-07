﻿using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyDataManager : IDataManager 
    {
        Strategy GetStrategy(int strategyId);

        List<Strategy> GetAllStrategies();

        BigResult<FraudResult> GetFilteredSuspiciousNumbers(int fromRow, int toRow, DateTime fromDate, DateTime toDate,  int? strategyId, string suspicionLevelsList);

        List<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description);

        bool AddStrategy(Strategy strategy, out int insertedId);

        bool UpdateStrategy(Strategy strategy);

        List<CallClass> GetAllCallClasses();

        List<Period> GetPeriods();

        List<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn);

        List<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber);
        
    }
}
