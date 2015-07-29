using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class GroupManager
    {
        public Vanrise.Entities.IDataRetrievalResult<Group> GetFilteredGroups(Vanrise.Entities.DataRetrievalInput<GroupQuery> input)
        {
            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredGroups(input));
        }

        public Group GetGroup(int groupId)
        {
            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            return dataManager.GetGroup(groupId);
        }

        public List<Group> GetGroups()
        {
            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            return dataManager.GetGroups();
        }

        public List<int> GetUserGroups(int userId)
        {
            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            return dataManager.GetUserGroups(userId);
        }

        public Vanrise.Entities.InsertOperationOutput<Group> AddGroup(Group groupObj, int[] members)
        {
            InsertOperationOutput<Group> insertOperationOutput = new InsertOperationOutput<Group>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int groupId = -1;

            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            bool insertActionSucc = dataManager.AddGroup(groupObj, out groupId);

            if(insertActionSucc)
            {
                dataManager.AssignMembers(groupId, members);
                
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                groupObj.GroupId = groupId;
                insertOperationOutput.InsertedObject = groupObj;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            
            return insertOperationOutput;
        }


        public Vanrise.Entities.UpdateOperationOutput<Group> UpdateGroup(Group groupObj, int[] members)
        {
            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            bool updateActionSucc = dataManager.UpdateGroup(groupObj);
            UpdateOperationOutput<Group> updateOperationOutput = new UpdateOperationOutput<Group>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                dataManager.AssignMembers(groupObj.GroupId, members);

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = groupObj;
            }
            return updateOperationOutput;
        }

    }
}
