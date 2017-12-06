using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class UserPasswordHistoryManager
    {
        public IEnumerable<UserPasswordHistory> GetUserPasswordHistoryByUserId(int userId, int maxRecordsCount)
        {
            IUserPasswordHistoryDataManager manager = SecurityDataManagerFactory.GetDataManager<IUserPasswordHistoryDataManager>();
            return manager.GetUserPasswordHistoryByUserId(userId, maxRecordsCount);
        }
        public bool AddUserPasswordHistory(UserPasswordHistory history, out int insertedId)
        {
            IUserPasswordHistoryDataManager manager = SecurityDataManagerFactory.GetDataManager<IUserPasswordHistoryDataManager>();
            return manager.AddUserPasswordHistory(history, out insertedId);

        }

        public void AddPasswordHistory(int userId, string hashedPassword, bool isChangedByAdmin)
        {
            int historyId;
            AddUserPasswordHistory(new UserPasswordHistory
            {
                IsChangedByAdmin = isChangedByAdmin,
                Password = hashedPassword,
                UserId = userId
            }, out historyId);
        }
    }
}
