using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IBusinessEntityDataManager : IDataManager
    {
        List<Vanrise.Security.Entities.BusinessEntity> GetEntities();

        bool ToggleBreakInheritance(string entityId);
    }
}
