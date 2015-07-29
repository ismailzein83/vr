﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class UserManager
    {
        public Vanrise.Entities.IDataRetrievalResult<User> GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<UserQuery> input)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredUsers(input));
        }

        public List<User> GetUsers()
        {
            IUserDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.GetUsers();
        }

        public List<User> GetMembers(int groupId)
        {
            IUserDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.GetMembers(groupId);
        }

        public User GetUserbyId(int userId)
        {
            IUserDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.GetUserbyId(userId);
        }

        public User GetUserbyEmail(string email)
        {
            IUserDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.GetUserbyEmail(email);
        }

        public Vanrise.Entities.InsertOperationOutput<User> AddUser(User userObject)
        {
            InsertOperationOutput<User> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<User>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int userId = -1;

            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            bool insertActionSucc = dataManager.AddUser(userObject, out userId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                userObject.UserId = userId;
                insertOperationOutput.InsertedObject = userObject;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<User> UpdateUser(User userObject)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            bool updateActionSucc = dataManager.UpdateUser(userObject);
            UpdateOperationOutput<User> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<User>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = userObject;
            }
            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<User> ResetPassword(int userId, string password)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            bool updateActionSucc = dataManager.ResetPassword(userId, password);

            UpdateOperationOutput<User> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<User>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GetUserbyId(userId);
            }

            return updateOperationOutput;
        }

        //public string EncodePassword(string password)
        //{
        //    return SecurityEssentials.PasswordEncryption.Encode(password);
        //}

        public bool CheckUserName(string name)
        {
            IUserDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.CheckUserName(name);
        }
    }
}
