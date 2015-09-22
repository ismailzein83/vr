using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class SwitchManager
    {
        public List<SwitchType> GetSwitchTypes()
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchTypes();
        }

        public Vanrise.Entities.IDataRetrievalResult<Switch> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSwitches(input));
        }

        public Switch GetSwitchByID(int switchID)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchByID(switchID);
        }

        public Switch GetSwitchByDataSourceID(int dataSourceID)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchByDataSourceID(dataSourceID);
        }

        public List<Switch> GetSwitches()
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitches();
        }

        public UpdateOperationOutput<Switch> UpdateSwitch(Switch switchObject)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool updated = dataManager.UpdateSwitch(switchObject);
            UpdateOperationOutput<Switch> updateOperationOutput = new UpdateOperationOutput<Switch>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = switchObject;
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public InsertOperationOutput<Switch> AddSwitch(Switch switchObject)
        {
            InsertOperationOutput<Switch> insertOperationOutput = new InsertOperationOutput<Switch>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int switchID = -1;

            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool inserted = dataManager.AddSwitch(switchObject, out switchID);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                switchObject.ID = switchID;
                insertOperationOutput.InsertedObject = switchObject;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
    }
}
