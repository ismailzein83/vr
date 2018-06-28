using Retail.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Data.Data
{
    public interface IUserSessionDataManager : IDataManager
    {
        List<UserSessionData> UpdateAndGetUserSessionData(List<UserSessionData> userSessionsData);
    }
}
