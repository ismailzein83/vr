using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace Demo.Module.Business
{
    public class UserManager
    {
        public IDataRetrievalResult<UserDetails> GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<UserQ> input)
        {
            IUserDataManager dataManager = DemoModuleFactory.GetDataManager<IUserDataManager>();
            IEnumerable<Demo.Module.Entities.User> users = dataManager.GetUsers();
            var allUsers = users.ToDictionary(u => u.Id, u => u);

            Func<Demo.Module.Entities.User, bool> filterExpression = (prod) =>
            {
                if (input.Query.Name != null && !prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };





            return DataRetrievalManager.Instance.ProcessResult(input, allUsers.ToBigResult(input, filterExpression, UserDetailMapper));
        }


        public Vanrise.Entities.InsertOperationOutput<UserDetails> AddUser(Demo.Module.Entities.User user)
        {
            Vanrise.Entities.InsertOperationOutput<UserDetails> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<UserDetails>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int userId = -1;

            IUserDataManager dataManager = DemoModuleFactory.GetDataManager<IUserDataManager>();
            bool insertActionSucc = dataManager.Insert(user, out userId);
            if (insertActionSucc)
            {

                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                user.Id = userId;
                insertOperationOutput.InsertedObject = UserDetailMapper(user);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Demo.Module.Entities.User GetUser(int Id)
        {
            IUserDataManager dataManager = DemoModuleFactory.GetDataManager<IUserDataManager>();
            
            
            Demo.Module.Entities.User u = dataManager.GetUser(Id);
           

            return u;
        }
        public Vanrise.Entities.UpdateOperationOutput<UserDetails> UpdateUser(Demo.Module.Entities.User user)
        {
            IUserDataManager dataManager = DemoModuleFactory.GetDataManager<IUserDataManager>();

            bool updateActionSucc = dataManager.Update(user);
            Vanrise.Entities.UpdateOperationOutput<UserDetails> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetails>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {

                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(user);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }


        public UserDetails UserDetailMapper(Demo.Module.Entities.User user)
        {
            UserDetails userDetail = new UserDetails();




            userDetail.Entity = user;

            return userDetail;
        }





    }
}
