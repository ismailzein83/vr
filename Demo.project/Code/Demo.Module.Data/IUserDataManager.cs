using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Demo.Module.Data
{
    public interface IUserDataManager : IDataManager
    {

        List<User> GetUsers();
        bool Update(User user);

       bool DeleteView(int Id);
        bool Insert(User user, out int insertedId);
        User GetUser(int Id);
        bool AreUsersUpdated(ref object updateHandle);
    }

}

