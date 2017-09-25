using System;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ICarrierAccountStatusHistoryDataManager : IDataManager
    {
        void Insert(int carrierAccountId, ActivationStatus status, ActivationStatus? previousStatus);
    }
}
