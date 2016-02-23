using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface ISystemActionDataManager : IDataManager
    {
        IEnumerable<SystemAction> GetSystemActions();

        bool AreSystemActionsUpdated(ref object updateHandle);
    }
}
