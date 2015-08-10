using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.BusinessEntity.Data;
using TOne.Entities;
using Vanrise.Entities;

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
            List<CarrierGroup> childEntities = entities.FindAll(x => x.ParentID.Value == groupId);
            CarrierGroupNode node = new CarrierGroupNode();

            if (childEntities.Count > 0)
            {
                if (node.Children == null)
                    node.Children = new List<CarrierGroupNode>();

                foreach (CarrierGroup item in childEntities)
                {
                    node.Children.Add(GetCarrierGroupNode(item, entities, node));
                }
            }

            retVal.Add(node.EntityId);


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

        public List<CarrierAccount> GetCarrierGroupMembers(int groupId, bool withDescendants)
        {
            List<CarrierAccount> lstCarrierAccounts = new List<CarrierAccount>();
            
            List<int> lstCarrierGroupIds = new List<int>();
            if (withDescendants)
                lstCarrierGroupIds = GetCarrierGroupIds(groupId);
            else
                lstCarrierGroupIds.Add(groupId);
            
            lstCarrierAccounts = _dataManager.GetCarrierGroupMembers(lstCarrierGroupIds);

            return lstCarrierAccounts;
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

        public List<CarrierInfo> GetCarriersInfoByGroup(List<CarrierAccount> lstCarrierAccount)
        {
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
