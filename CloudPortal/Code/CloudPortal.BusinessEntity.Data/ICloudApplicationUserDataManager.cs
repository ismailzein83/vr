using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Data
{
    public interface ICloudApplicationUserDataManager : IDataManager
    {
        bool AreApplicationUsersUpdated(ref object updateHandle);

        List<Entities.CloudApplicationUser> GetAllApplicationUsers();

        bool AddApplicationUser(Entities.CloudApplicationUser applicationUser);

        bool UpdateApplicationUser(Entities.CloudApplicationUser applicationUser);
    }
}
