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

        public List<CarrierGroupNode> GetEntityNodes()
        {
            List<CarrierGroup> entities = _dataManager.GetEntities();

            List<CarrierGroupNode> retVal = new List<CarrierGroupNode>();

            List<CarrierGroup> childEntities = entities.FindAll(x => !x.ParentID.HasValue);
            CarrierGroupNode node = new CarrierGroupNode() { EntityId = 0, Name = "Root", Parent = null};

            if (childEntities.Count > 0)
            {
                if (node.Children == null)
                    node.Children = new List<CarrierGroupNode>();

                foreach (CarrierGroup item in childEntities)
                {
                    node.Children.Add(GetModuleNode(item, entities, node));
                }
            }

            retVal.Add(node);
      

            return retVal;
        }

        public TOne.Entities.InsertOperationOutput<CarrierGroup> AddGroup(CarrierGroup groupObj, string[] members)
        {
            TOne.Entities.InsertOperationOutput<CarrierGroup> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CarrierGroup>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            CarrierGroup carrierParent;
            if (groupObj.ParentID == 0)
                groupObj.ParentID = null;

            if (groupObj.ParentID != null)
            {
                carrierParent = GetCarrierGroup(groupObj.ParentID.Value);

                if (carrierParent.ParentPath == null || carrierParent.ParentPath == "")
                    groupObj.ParentPath = carrierParent.CarrierGroupName;
                else
                    groupObj.ParentPath = carrierParent.ParentPath + "/" + carrierParent.CarrierGroupName;
            }
            
            int carrierGroupId;
            bool insertActionSucc =  _dataManager.AddCarrierGroup(groupObj, out carrierGroupId);
            CarrierAccount ca = new CarrierAccount();
            for(int i = 0; i < members.Count(); i++)
            {
                ca = _dataAccountManager.GetCarrierAccount(members[i]);
                ca.CarrierAccountId = members[i];
                ca.CarrierGroupID = carrierGroupId;
                if (ca.CarrierGroups == null || ca.CarrierGroups == "")
                    ca.CarrierGroups = carrierGroupId.ToString();
                else
                    ca.CarrierGroups = ca.CarrierGroups  + "," + carrierGroupId.ToString();
                _dataAccountManager.UpdateCarrierAccountGroup(ca);
            }


            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = groupObj;
            }
            return insertOperationOutput;

        }

        public TOne.Entities.UpdateOperationOutput<CarrierGroup> UpdateGroup(CarrierGroup groupObj, string[] members)
        {
            TOne.Entities.UpdateOperationOutput<CarrierGroup> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierGroup>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            CarrierGroup carrierParent;
            CarrierGroup oldCarrier;
            if (groupObj.ParentID == 0)
                groupObj.ParentID = null;

            if (groupObj.ParentID != null)
            {
                oldCarrier = GetCarrierGroup(groupObj.CarrierGroupID);
                if (groupObj.ParentID != oldCarrier.CarrierGroupID)
                {
                    carrierParent = GetCarrierGroup(groupObj.ParentID.Value);
                    if (carrierParent.ParentPath == null || carrierParent.ParentPath == "")
                        groupObj.ParentPath = carrierParent.CarrierGroupName;
                    else
                        groupObj.ParentPath = carrierParent.ParentPath + "/" + carrierParent.CarrierGroupName;
                }
                else
                {
                    groupObj.ParentID = oldCarrier.ParentID;
                    groupObj.ParentPath = oldCarrier.ParentPath;
                }
            }

            bool updateActionSucc = _dataManager.UpdateCarrierGroup(groupObj);

            //remove the old group
            List<CarrierAccount> lstCarrierAccount = _dataAccountManager.GetAllCarrierAccounts().Values.ToList<CarrierAccount>();
            for(int i = 0; i< lstCarrierAccount.Count(); i++)
            {
                if (lstCarrierAccount[i].CarrierGroupID == groupObj.CarrierGroupID)
                {
                    lstCarrierAccount[i].CarrierGroupID = null;
                }
                
                if (lstCarrierAccount[i].GroupIds != null)
                    lstCarrierAccount[i].GroupIds = lstCarrierAccount[i].GroupIds.Where(c => c != groupObj.CarrierGroupID).ToList();
                
                if (lstCarrierAccount[i].GroupIds != null)
                {
                    lstCarrierAccount[i].CarrierGroups = "";
                    if (lstCarrierAccount[i].GroupIds.Count() == 1)
                        lstCarrierAccount[i].CarrierGroups = lstCarrierAccount[i].GroupIds[0].ToString();
                    else
                    {
                        for (int k = 0; k < lstCarrierAccount[i].GroupIds.Count(); k++ )
                        {
                            lstCarrierAccount[i].CarrierGroups = lstCarrierAccount[i].CarrierGroups + lstCarrierAccount[i].GroupIds[k].ToString() + ",";
                        }
                        if (lstCarrierAccount[i].CarrierGroups.Length > 1)
                            lstCarrierAccount[i].CarrierGroups = lstCarrierAccount[i].CarrierGroups.Remove(lstCarrierAccount[i].CarrierGroups.Length - 1);
                    }
                }
                _dataAccountManager.UpdateCarrierAccountGroup(lstCarrierAccount[i]);
            }

            //Insert the new group
            CarrierAccount ca = new CarrierAccount();
            for(int i = 0; i < members.Count(); i++)
            {
                ca = _dataAccountManager.GetCarrierAccount(members[i]);
                ca.CarrierAccountId = members[i];
                ca.CarrierGroupID = groupObj.CarrierGroupID;
                if (ca.CarrierGroups == null || ca.CarrierGroups == "")
                    ca.CarrierGroups = groupObj.CarrierGroupID.ToString();
                else
                    ca.CarrierGroups = ca.CarrierGroups  + "," + groupObj.CarrierGroupID.ToString();
                _dataAccountManager.UpdateCarrierAccountGroup(ca);
            }

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = groupObj;
            }
            return updateOperationOutput;
        }


        public List<CarrierGroup> GetCarrierGroups()
        {
            return _dataManager.GetEntities();
        }
        private CarrierGroupNode GetModuleNode(CarrierGroup module, List<CarrierGroup> entities, CarrierGroupNode parent)
        {
            CarrierGroupNode node = new CarrierGroupNode()
            {
                EntityId = module.CarrierGroupID,
                Name = module.CarrierGroupName,
                Parent = parent ?? new CarrierGroupNode() { EntityId = 0, Name = "Root", Parent = null},
            };

            List<CarrierGroup> childEntities = entities.FindAll(x => x.ParentID == module.CarrierGroupID);

            if (childEntities.Count > 0)
            {
                if (node.Children == null)
                    node.Children = new List<CarrierGroupNode>();

                foreach (CarrierGroup item in childEntities)
                {
                    node.Children.Add(GetModuleNode(item, entities, node));
                }
            }

            return node;
        }

        public List<CarrierAccount> GetCarriersByGroup(string groupId)
        {
            List<CarrierAccount> OldRelatedCarrierAccounts = new List<CarrierAccount>();

            Dictionary<string, CarrierAccount> lstCarrierAccounts = _dataAccountManager.GetAllCarrierAccounts();
            foreach (CarrierAccount ItemAccount in lstCarrierAccounts.Values)
            {
                if (ItemAccount.GroupIds != null)
                {
                    foreach (int carrierGroup in ItemAccount.GroupIds)
                    {
                        if (carrierGroup == int.Parse(groupId))
                        {
                            OldRelatedCarrierAccounts.Add(ItemAccount);
                        }
                    }
                }
            }
            return OldRelatedCarrierAccounts;
        }

        public List<CarrierInfo> GetCarriersInfoByGroup(List<CarrierAccount> lstCarrierAccount)
        {
            List<CarrierInfo> lstCarrierInfo = new List<CarrierInfo>();
            foreach(CarrierAccount ca in lstCarrierAccount )
            {
                lstCarrierInfo.Add(new CarrierInfo(){
                    CarrierAccountID = ca.CarrierAccountId,
                    Name = string.Format("{0}{1}", ca.ProfileName, string.IsNullOrEmpty(ca.NameSuffix) ? string.Empty : " (" + ca.NameSuffix + ")")
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
