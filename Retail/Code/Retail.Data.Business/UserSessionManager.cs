using Retail.Data.Data;
using Retail.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Data.Business
{
    public class UserSessionManager
    {
        public List<UserSessionData> UpdateAndGetUserSessionData(List<UserSessionData> userSessionDataList)
        {
            IUserSessionDataManager userSessionDataManager = DataDataManagerFactory.GetDataManager<IUserSessionDataManager>();
            return userSessionDataManager.UpdateAndGetUserSessionData(userSessionDataList);
        }
    }
}
