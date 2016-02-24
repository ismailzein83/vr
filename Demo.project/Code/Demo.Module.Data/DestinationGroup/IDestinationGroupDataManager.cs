using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface IDestinationGroupDataManager:IDataManager
    {
        List<DestinationGroup> GetDestinationGroups();
        bool Insert(DestinationGroup group, out int groupId);
        bool Update(DestinationGroup group);
        bool AreDestinationGroupsUpdated(ref object updateHandle);
    }
}
