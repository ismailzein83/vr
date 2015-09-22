using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class SwitchTrunkManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SwitchTrunkDetail> GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkDetailQuery> input)
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSwitchTrunks(input));
        }

        public SwitchTrunkDetail GetSwitchTrunkByID(int trunkID)
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return dataManager.GetSwitchTrunkByID(trunkID);
        }

        public InsertOperationOutput<SwitchTrunkDetail> AddSwitchTrunk(SwitchTrunk trunkObject)
        {
            InsertOperationOutput<SwitchTrunkDetail> insertOperationOutput = new InsertOperationOutput<SwitchTrunkDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int trunkID = -1;

            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            bool inserted = dataManager.AddSwitchTrunk(trunkObject, out trunkID);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = dataManager.GetSwitchTrunkByID(trunkID);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<SwitchTrunkDetail> UpdateSwitchTrunk(SwitchTrunk trunkObject)
        {
            UpdateOperationOutput<SwitchTrunkDetail> updateOperationOutput = new UpdateOperationOutput<SwitchTrunkDetail>();
            
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            bool updated = dataManager.UpdateSwitchTrunk(trunkObject);

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = dataManager.GetSwitchTrunkByID(trunkObject.ID);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
    }
}
