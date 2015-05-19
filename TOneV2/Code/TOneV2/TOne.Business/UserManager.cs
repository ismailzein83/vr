using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;
using TOne.Data;

namespace TOne.Business
{
    public class UserManager
    {
        public List<TOne.Entities.User> GetUsers(int FromRow, int ToRow)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.GetUsers(FromRow, ToRow);
        }

        public void DeleteUser(int Id)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            datamanager.DeleteUser(Id);
        }

        public User AddUser(User user)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.AddUser(user);
        }

        public bool UpdateUser(User user)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.UpdateUser(user);
        }

        public List<TOne.Entities.User> SearchUser(string name, string email)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.SearchUser(name, email);
        }

        public string EncodePassword(string password)
        {
            return SecurityEssentials.PasswordEncryption.Encode(password);
        }
    }
}
