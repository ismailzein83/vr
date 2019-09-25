using System;
using Retail.BusinessEntity.Data;

namespace NetworkProvision.Data
{
    public interface IActionHandlerDataManager : IDataManager
    {
        bool AreActionHandlersUpdated(ref object updateHandle);
    }
}
