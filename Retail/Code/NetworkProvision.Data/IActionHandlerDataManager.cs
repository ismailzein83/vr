using System;

namespace NetworkProvision.Data
{
    public interface IActionHandlerDataManager : IDataManager
    {
        bool AreActionHandlersUpdated(ref object updateHandle);
    }
}
