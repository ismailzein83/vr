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
        public List<TOne.Entities.User> GetFilteredUsers(int fromRow, int toRow, string name, string email)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.GetFilteredUsers(fromRow, toRow, name, email);
        }

        public User GetUser(int userId)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.GetUser(userId);
        }

        public TOne.Entities.OperationResults.InsertOperationOutput<User> AddUser(User userObject)
        {
            //IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            //return datamanager.AddUser(user);


            TOne.Entities.OperationResults.InsertOperationOutput<User> insertOperationOutput = new TOne.Entities.OperationResults.InsertOperationOutput<User>();

            insertOperationOutput.Result = TOne.Entities.OperationResults.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int userId = -1;

            IUserDataManager dataManager = DataManagerFactory.GetDataManager<IUserDataManager>();
            bool insertActionSucc = dataManager.AddUser(userObject, out userId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = TOne.Entities.OperationResults.InsertOperationResult.Succeeded;
                userObject.UserId = userId;
                insertOperationOutput.InsertedObject = userObject;
            }
            return insertOperationOutput;


        }

        public TOne.Entities.UpdateOperationOutput<User> UpdateUser(User userObject)
        {
            IUserDataManager dataManager = DataManagerFactory.GetDataManager<IUserDataManager>();
            bool updateActionSucc = dataManager.UpdateUser(userObject);
            TOne.Entities.UpdateOperationOutput<User> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<User>();

            updateOperationOutput.Result = TOne.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = TOne.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = userObject;
            }
            return updateOperationOutput;
        }

        public bool ResetPassword(int userId, string password)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.ResetPassword(userId, password);
        }

        public string EncodePassword(string password)
        {
            return SecurityEssentials.PasswordEncryption.Encode(password);
        }

        public bool CheckUserName(string name)
        {
            IUserDataManager datamanager = DataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.CheckUserName(name); 
        }
    }
}
