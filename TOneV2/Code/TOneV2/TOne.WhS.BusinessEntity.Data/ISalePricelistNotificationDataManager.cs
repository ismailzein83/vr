using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISalePricelistNotificationDataManager : IDataManager
    {
        bool Insert(int pricelitsId, int customerId);
        IEnumerable<SalePricelistNotification> GetSalePricelistNotifications();
        Dictionary<int, int> GetNotificationCountByPricelistId();
        bool AreSalePriceListNotificationUpdated(ref object updateHandle);
        IEnumerable<SalePricelistNotification> GetLastSalePricelistNotification(IEnumerable<int> customerIds);
    }
}
