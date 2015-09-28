using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class SwitchTypeManager
    {
        public List<SwitchType> GetSwitchTypes()
        {
            ISwitchTypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTypeDataManager>();
            return dataManager.GetSwitchTypes();
        }

        public Vanrise.Entities.IDataRetrievalResult<SwitchType> GetFilteredSwitchTypes(Vanrise.Entities.DataRetrievalInput<SwitchTypeQuery> input)
        {
            ISwitchTypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTypeDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSwitchTypes(input));
        }

        public SwitchType GetSwitchTypeByID(int switchTypeID)
        {
            ISwitchTypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTypeDataManager>();
            return dataManager.GetSwitchTypeByID(switchTypeID);
        }

        public InsertOperationOutput<SwitchType> AddSwitchType(SwitchType switchTypeObject)
        {
            InsertOperationOutput<SwitchType> insertOperationOutput = new InsertOperationOutput<SwitchType>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int switchTypeID = -1;

            ISwitchTypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTypeDataManager>();
            bool inserted = dataManager.AddSwitchType(switchTypeObject, out switchTypeID);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                switchTypeObject.ID = switchTypeID;
                insertOperationOutput.InsertedObject = switchTypeObject;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<SwitchType> UpdateSwitchType(SwitchType switchTypeObject)
        {
            UpdateOperationOutput<SwitchType> updateOperationOutput = new UpdateOperationOutput<SwitchType>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ISwitchTypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTypeDataManager>();
            bool updated = dataManager.UpdateSwitchType(switchTypeObject);

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = switchTypeObject;
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteSwitchType(int switchTypeID)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.InUse;

            ISwitchTypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTypeDataManager>();
            bool deleted = dataManager.DeleteSwitchType(switchTypeID);

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
