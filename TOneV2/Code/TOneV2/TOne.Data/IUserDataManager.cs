using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;

namespace TOne.Data
{
    public interface IUserDataManager : IDataManager
    {
        List<User> GetUsers(int FromRow, int ToRow);

        void DeleteUser(int Id);

        User AddUser(User user);

        bool UpdateUser(User user);

        List<Entities.User> SearchUser(string name, string email);
    }
}
