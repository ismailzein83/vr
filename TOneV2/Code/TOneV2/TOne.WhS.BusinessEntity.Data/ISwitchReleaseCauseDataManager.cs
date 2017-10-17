using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISwitchReleaseCauseDataManager : IDataManager
    {

        List<Entities.SwitchReleaseCause> GetSwitchReleaseCauses();
        bool AddSwitchReleaseCause(SwitchReleaseCause switchReleaseCause, out int insertedId);
        bool UpdateSwitchReleaseCause(SwitchReleaseCause switchReleaseCause);
        bool AreSwitchReleaseCausesUpdated(ref object updateHandle);
        string GetSwitchReleaseCauseName(int switchReleaseCauseId);
    }
}
