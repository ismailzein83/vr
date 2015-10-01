using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class SwitchManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSwitches(input));
        }

        public SwitchDetail GetSwitchByID(int switchID)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchByID(switchID);
        }

        public Switch GetSwitchByDataSourceID(int dataSourceID)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchByDataSourceID(dataSourceID);
        }

        public List<SwitchInfo> GetSwitches()
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitches();
        }

        public List<SwitchInfo> GetSwitchesToLinkTo(int switchID)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchesToLinkTo(switchID);
        }

        public UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch switchObject)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool updated = dataManager.UpdateSwitch(switchObject);
            UpdateOperationOutput<SwitchDetail> updateOperationOutput = new UpdateOperationOutput<SwitchDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = dataManager.GetSwitchByID(switchObject.ID);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public InsertOperationOutput<SwitchDetail> AddSwitch(Switch switchObject)
        {
            InsertOperationOutput<SwitchDetail> insertOperationOutput = new InsertOperationOutput<SwitchDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int switchID = -1;

            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool inserted = dataManager.AddSwitch(switchObject, out switchID);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                switchObject.ID = switchID;
                insertOperationOutput.InsertedObject = dataManager.GetSwitchByID(switchObject.ID);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteSwitch(int switchID)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.InUse;

            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool deleted = dataManager.DeleteSwitch(switchID);

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
