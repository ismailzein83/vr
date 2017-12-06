using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IUserPasswordHistoryDataManager : IDataManager
    {
        IEnumerable<UserPasswordHistory> GetUserPasswordHistoryByUserId(int userId, int maxRecordsCount);

        bool AddUserPasswordHistory(UserPasswordHistory history, out int insertedId);
    }
}
