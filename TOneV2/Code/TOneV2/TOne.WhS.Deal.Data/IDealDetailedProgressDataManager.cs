using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
    public interface IDealDetailedProgressDataManager : IDataManager
    {
        List<DealDetailedProgress> GetDealDetailedProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale, DateTime? beginDate, DateTime? endDate);

        List<DealDetailedProgress> GetDealsDetailedProgress(List<int> dealIds, DateTime fromDate, DateTime toDate);
        List<DealDetailedProgress> GetDealDetailedProgressesByDate(bool isSale, DateTime? beginDate, DateTime? endDate);

        void InsertDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses);

        void UpdateDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses);

        void DeleteDealDetailedProgresses(List<long> dealDetailedProgressIds);

        DateTime? GetDealEvaluatorBeginDate(byte[] lastTimestamp);

        Byte[] GetMaxTimestamp();

        void DeleteDealDetailedProgresses(bool isSale, DateTime? beginDate, DateTime? endDate);
    }
}