using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Entities;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace TOne.BusinessEntity.Business
{
    public class CarrierGroupManager
    {
        ICarrierGroupDataManager _dataManager;
        ICarrierAccountDataManager _dataAccountManager;

        public CarrierGroupManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ICarrierGroupDataManager>();
            _dataAccountManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
        }

        public List<int> GetCarrierGroupIds(int groupId)
        {
            List<CarrierGroup> entities = _dataAccountManager.GetAllCarrierGroups().Values.ToList<CarrierGroup>();

            List<int> retVal = new List<int>();
            retVal.Add(groupId);
            CarrierGroup parent = entities.FindAll(x => x.ID == groupId).FirstOrDefault();
            
            retVal.AddRange(GetChildrenMembers(parent, entities));

            return retVal;
        }
        private List<int> GetChildrenMembers(CarrierGroup parent, List<CarrierGroup> entities)
        {
            List<int> retVal = new List<int>();
            List<CarrierGroup> ent =  entities.FindAll(x => (x.ParentID.HasValue) && (x.ParentID.Value == parent.ID));
            foreach(CarrierGroup cg in ent)
            {
                retVal.Add(cg.ID);
                retVal.AddRange(GetChildrenMembers(cg, entities));
            }
            return retVal;
        }

        public List<CarrierGroupNode> GetEntityNodes()
        {
            List<CarrierGroup> entities = _dataAccountManager.GetAllCarrierGroups().Values.ToList<CarrierGroup>();

            List<CarrierGroupNode> retVal = new List<CarrierGroupNode>();

            List<CarrierGroup> childEntities = entities.FindAll(x => !x.ParentID.HasValue);
            CarrierGroupNode node = new CarrierGroupNode() { EntityId = 0, Name = "Root", Parent = null};

            if (childEntities.Count > 0)
            {
                if (node.Children == null)
                    node.Children = new List<CarrierGroupNode>();

                foreach (CarrierGroup item in childEntities)
                {
                    node.Children.Add(GetCarrierGroupNode(item, entities, node));
                }
            }

            retVal.Add(node);
      

            return retVal;
        }
        
        private CarrierGroupNode GetCarrierGroupNode(CarrierGroup module, List<CarrierGroup> entities, CarrierGroupNode parent)
        {
            CarrierGroupNode node = new CarrierGroupNode()
            {
                EntityId = module.ID,
                Name = module.Name,
                Parent = parent ?? new CarrierGroupNode() { EntityId = 0, Name = "Root", Parent = null },
            };

            List<CarrierGroup> childEntities = entities.FindAll(x => x.ParentID == module.ID);

            if (childEntities.Count > 0)
            {
                if (node.Children == null)
                    node.Children = new List<CarrierGroupNode>();

                foreach (CarrierGroup item in childEntities)
                {
                    node.Children.Add(GetCarrierGroupNode(item, entities, node));
                }
            }

            return node;
        }

        private List<int> GetCarrierGroupIds(int groupId, bool withDescendants)
        {
            List<int> lstCarrierGroupIds = new List<int>();
            if (withDescendants)
                lstCarrierGroupIds = GetCarrierGroupIds(groupId);
            else
                lstCarrierGroupIds.Add(groupId);
            return lstCarrierGroupIds;
        }

        public List<CarrierAccount> GetCarrierGroupMembers(int groupId, bool withDescendants)
        {
            return _dataManager.GetCarrierGroupMembers(GetCarrierGroupIds(groupId, withDescendants));
        }

        public Vanrise.Entities.IDataRetrievalResult<CarrierAccount> GetCarrierGroupMembers(Vanrise.Entities.DataRetrievalInput<CarrierGroupQuery> input)
        {
            if (input.Query.WithAssignedCarrier && input.Query.CarrierType == CarrierType.Customer)
            {
                AccountManagerManager accountManagerManager = new AccountManagerManager();
                List<AssignedCarrier> assignedCarriers = accountManagerManager.GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierType.Customer);
                List<string> cutomers = new List<string>();
                foreach (AssignedCarrier assignedCarrier in assignedCarriers)
                {
                    cutomers.Add(assignedCarrier.CarrierAccountId);
                }
                return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetCarrierGroupMembers(input, GetCarrierGroupIds(input.Query.GroupId, input.Query.WithDescendants), cutomers));
            }
            else if (input.Query.WithAssignedCarrier && input.Query.CarrierType == CarrierType.Supplier)
            {
                AccountManagerManager accountManagerManager = new AccountManagerManager();
                List<AssignedCarrier> assignedCarriers = accountManagerManager.GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierType.Supplier);
                List<string> suppliers = new List<string>();
                foreach (AssignedCarrier assignedCarrier in assignedCarriers)
                {
                    suppliers.Add(assignedCarrier.CarrierAccountId);
                }
                return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetCarrierGroupMembers(input, GetCarrierGroupIds(input.Query.GroupId, input.Query.WithDescendants), suppliers));
            }

            else
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetCarrierGroupMembers(input, GetCarrierGroupIds(input.Query.GroupId, input.Query.WithDescendants),null));
        }

        public TOne.Entities.InsertOperationOutput<CarrierGroup> AddGroup(CarrierGroup groupObj, string[] CarrierAccountIds)
        {
            TOne.Entities.InsertOperationOutput<CarrierGroup> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CarrierGroup>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            //CarrierGroup carrierParent;
            if (groupObj.ParentID == 0)
                groupObj.ParentID = null;

            int carrierGroupId;
            bool insertActionSucc = _dataManager.AddCarrierGroup(groupObj, CarrierAccountIds, out carrierGroupId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = groupObj;
            }
            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<CarrierGroup> UpdateGroup(CarrierGroup groupObj, string[] CarrierAccountIds)
        {
            TOne.Entities.UpdateOperationOutput<CarrierGroup> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierGroup>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            CarrierGroup oldCarrier;
            if (groupObj.ParentID == 0)
                groupObj.ParentID = null;

            if (groupObj.ParentID != null)
            {
                oldCarrier = GetCarrierGroup(groupObj.ID);
                if (groupObj.ParentID == oldCarrier.ID)
                    groupObj.ParentID = oldCarrier.ParentID;
            }

            bool updateActionSucc = _dataManager.UpdateCarrierGroup(groupObj, CarrierAccountIds);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = groupObj;
            }
            return updateOperationOutput;
        }

        public List<CarrierInfo> GetCarriersInfoByGroup(int groupId, bool withDescendants)
        {
            List<CarrierAccount>  lstCarrierAccount = GetCarrierGroupMembers(groupId, withDescendants);
            List<CarrierInfo> lstCarrierInfo = new List<CarrierInfo>();
            foreach (CarrierAccount ca in lstCarrierAccount)
            {
                lstCarrierInfo.Add(new CarrierInfo()
                {
                    CarrierAccountID = ca.CarrierAccountId,
                    Name = ca.CarrierAccountName,
                });
            }

            return lstCarrierInfo;
        }

        public CarrierGroup GetCarrierGroup(int carrierGroupId)
        {
            return _dataManager.GetCarrierGroup(carrierGroupId);
        }
    }
}
