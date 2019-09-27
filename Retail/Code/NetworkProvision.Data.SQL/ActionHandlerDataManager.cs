using System;
using Vanrise.Data.SQL;

namespace NetworkProvision.Data.SQL
{
    public class ActionHandlerDataManager : BaseSQLDataManager,  IActionHandlerDataManager
    {
        public bool AreActionHandlersUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }
    }
}