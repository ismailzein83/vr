using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        List<Switch> GetSwitches();

        bool Insert(Switch whsSwitch, out int insertedId);

        bool Update(Switch whsSwitch);

        bool Delete(int switchId);

        bool AreSwitchesUpdated(ref object updateHandle);

    }
}
