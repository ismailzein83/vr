﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
    public interface IDealDetailedProgressDataManager : IDataManager
    {
        List<DealDetailedProgress> GetDealDetailedProgresses(List<DealZoneGroup> dealZoneGroups);

        List<DealDetailedProgress> GetDealDetailedProgresses(bool isSale, DateTime beginDate);

        void InsertDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses);

        void UpdateDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses);

        void DeleteDealDetailedProgresses(List<long> dealDetailedProgressIds);

        DateTime? GetDealEvaluatorBeginDate(byte[] lastTimestamp);

        List<DealZoneGroupData> GetDealZoneGroupDataBeforeDate(bool isSale, DateTime beforeDate);

        List<DealZoneGroupTierData> GetDealZoneGroupTierDataBeforeDate(bool isSale, DateTime beforeDate, List<DealZoneGroupTierRate> dealZoneGroupTierRates);
    }
}
