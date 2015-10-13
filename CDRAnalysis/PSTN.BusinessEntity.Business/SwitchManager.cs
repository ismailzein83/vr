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

        public SwitchDetail GetSwitchById(int switchId)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchById(switchId);
        }

        public Switch GetSwitchByDataSourceId(int dataSourceId)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchByDataSourceId(dataSourceId);
        }

        public List<SwitchInfo> GetSwitches()
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitches();
        }

        public List<SwitchInfo> GetSwitchesToLinkTo(int switchId)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchesToLinkTo(switchId);
        }

        public List<SwitchInfo> GetSwitchesByIds(List<int> switchIds)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchesByIds(switchIds);
        }

        public List<SwitchAssignedDataSource> GetSwitchAssignedDataSources()
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchAssignedDataSources();
        }

        public UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch switchObj)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool updated = dataManager.UpdateSwitch(switchObj);
            UpdateOperationOutput<SwitchDetail> updateOperationOutput = new UpdateOperationOutput<SwitchDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = dataManager.GetSwitchById(switchObj.SwitchId);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public InsertOperationOutput<SwitchDetail> AddSwitch(Switch switchObj)
        {
            InsertOperationOutput<SwitchDetail> insertOperationOutput = new InsertOperationOutput<SwitchDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int switchId = -1;

            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool inserted = dataManager.AddSwitch(switchObj, out switchId);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                switchObj.SwitchId = switchId;
                insertOperationOutput.InsertedObject = dataManager.GetSwitchById(switchObj.SwitchId);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteSwitch(int switchId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.InUse;

            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool deleted = dataManager.DeleteSwitch(switchId);

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
