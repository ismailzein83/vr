using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;

namespace TOne.Data
{
    public interface ISecurityDataManager : IDataManager
    {
        List<User> GetUsers(int DisplayStart, int DisplayLength);

        void DeleteUser(int Id);

        void AddUser(User user);

        void EditUser(User user);

        List<Entities.User> SearchUser(string name, string email);
    }
}
