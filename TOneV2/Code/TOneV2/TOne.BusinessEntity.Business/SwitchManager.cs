﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class SwitchManager
    {
        public List<SwitchInfo> GetSwitches()
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitches();
        }


        public List<Switch> GetFilteredSwitches(string switchName, int rowFrom, int rowTo)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetFilteredSwitches(switchName, rowFrom, rowTo);
        }

        public Switch GetSwitchDetails(int switchID)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchDetails(switchID);
        }

        public TOne.Entities.UpdateOperationOutput<Switch> UpdateSwitch(Switch switchObject)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool updateActionSucc = dataManager.UpdateSwitch(switchObject);
            TOne.Entities.UpdateOperationOutput<Switch> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<Switch>();

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = switchObject;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            return updateOperationOutput;
        }


        public TOne.Entities.InsertOperationOutput<Switch> InsertSwitch(Switch switchObject)
        {
            TOne.Entities.InsertOperationOutput<Switch> insertOperationOutput = new TOne.Entities.InsertOperationOutput<Switch>();

            int switchId = -1;

            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool insertActionSucc = dataManager.InsertSwitch(switchObject, out switchId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                switchObject.SwitchId = switchId;
                insertOperationOutput.InsertedObject = switchObject;
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            return insertOperationOutput;
        }
        public string GetSwitchName(int switchId)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchName(switchId);
        }
    }
}
