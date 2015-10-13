using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class TypeManager
    {
        public List<Type> GetTypes()
        {
            ITypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITypeDataManager>();
            return dataManager.GetTypes();
        }

        public Vanrise.Entities.IDataRetrievalResult<Type> GetFilteredTypes(Vanrise.Entities.DataRetrievalInput<TypeQuery> input)
        {
            ITypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITypeDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredTypes(input));
        }

        public Type GetTypeById(int switchTypeId)
        {
            ITypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITypeDataManager>();
            return dataManager.GetTypeById(switchTypeId);
        }

        public InsertOperationOutput<Type> AddType(Type switchTypeObj)
        {
            InsertOperationOutput<Type> insertOperationOutput = new InsertOperationOutput<Type>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int switchTypeId = -1;

            ITypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITypeDataManager>();
            bool inserted = dataManager.AddType(switchTypeObj, out switchTypeId);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                switchTypeObj.TypeId = switchTypeId;
                insertOperationOutput.InsertedObject = switchTypeObj;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<Type> UpdateType(Type switchTypeObj)
        {
            UpdateOperationOutput<Type> updateOperationOutput = new UpdateOperationOutput<Type>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ITypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITypeDataManager>();
            bool updated = dataManager.UpdateType(switchTypeObj);

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = switchTypeObj;
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteType(int switchTypeId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.InUse;

            ITypeDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITypeDataManager>();
            bool deleted = dataManager.DeleteType(switchTypeId);

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
