using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Data;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    public class DealProgressManager
    {
        #region Public Methods

        public Dictionary<DealZoneGroup, DealProgress> GetDealProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            IDealProgressDataManager dealProgressDataManager = DealDataManagerFactory.GetDataManager<IDealProgressDataManager>();
            List<DealProgress> dealProgressList = dealProgressDataManager.GetDealProgresses(dealZoneGroups, isSale);
            if (dealProgressList == null || dealProgressList.Count == 0)
                return null;

            return dealProgressList.ToDictionary(itm => new DealZoneGroup() { DealId = itm.DealID, ZoneGroupNb = itm.ZoneGroupNb }, itm => itm);
        }

        public void InsertDealProgresses(IEnumerable<DealProgress> dealProgresses)
        {
            if (dealProgresses == null || dealProgresses.Count() == 0)
                return;

            IDealProgressDataManager dealProgressDataManager = DealDataManagerFactory.GetDataManager<IDealProgressDataManager>();
            dealProgressDataManager.InsertDealProgresses(dealProgresses.ToList());
        }

        public void UpdateDealProgresses(IEnumerable<DealProgress> dealProgresses)
        {
            if (dealProgresses == null || dealProgresses.Count() == 0)
                return;

            IDealProgressDataManager dealProgressDataManager = DealDataManagerFactory.GetDataManager<IDealProgressDataManager>();
            dealProgressDataManager.UpdateDealProgresses(dealProgresses.ToList());
        }

        #endregion
    }
}