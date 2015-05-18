using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;

namespace TOne.Data
{
    public interface ISecurityDataManager : IDataManager
    {
        List<User> GetUsers();

        void DeleteUser(int Id);

        void SaveUser(User user);

        List<Entities.User> SearchUser(string name, string email);
    }
}
