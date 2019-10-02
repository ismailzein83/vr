using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISalePricelistNotificationDataManager : IDataManager
    {
        bool Insert(int customerId, int pricelitsId, long fileId);
        IEnumerable<SalePricelistNotification> GetSalePricelistNotifications();
        Dictionary<int, int> GetNotificationCountByPricelistId();
        bool AreSalePriceListNotificationUpdated(ref object updateHandle);
        IEnumerable<SalePricelistNotification> GetLastSalePricelistNotifications(IEnumerable<int> customerIds);
        IEnumerable<SalePricelistNotification> GetSalePricelistNotifictaions(int pricelistId);
    }
}
