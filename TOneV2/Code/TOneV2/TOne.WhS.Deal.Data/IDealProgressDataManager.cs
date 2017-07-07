using System.Collections.Generic;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data
{
    public interface IDealProgressDataManager : IDataManager
    {
        List<DealProgress> GetDealProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale);

        void UpdateDealProgresses(List<DealProgress> dealProgresses);

        void InsertDealProgresses(List<DealProgress> dealProgresses);

        void DeleteDealProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale);

        IEnumerable<DealZoneGroup> GetAffectedDealZoneGroups(bool isSale);

        void InsertAffectedDealZoneGroups(HashSet<DealZoneGroup> dealZoneGroups, bool isSale);

        void DeleteAffectedDealZoneGroups();
    }
}