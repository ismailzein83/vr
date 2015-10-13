using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class TrunkManager
    {
        public Vanrise.Entities.IDataRetrievalResult<TrunkDetail> GetFilteredTrunks(Vanrise.Entities.DataRetrievalInput<TrunkQuery> input)
        {
            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredTrunks(input));
        }

        public TrunkDetail GetTrunkById(int trunkId)
        {
            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            return dataManager.GetTrunkById(trunkId);
        }

        public TrunkInfo GetTrunkBySymbol(string symbol)
        {
            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            return dataManager.GetTrunkBySymbol(symbol);
        }

        public List<TrunkInfo> GetTrunksBySwitchIds(TrunkFilter trunkFilterObj)
        {
            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            return dataManager.GetTrunksBySwitchIds(trunkFilterObj);
        }

        public List<TrunkInfo> GetTrunks()
        {
            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            return dataManager.GetTrunks();
        }

        public List<TrunkInfo> GetTrunksByIds(List<int> trunkIds)
        {
            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            return dataManager.GetTrunksByIds(trunkIds);
        }

        public InsertOperationOutput<TrunkDetail> AddTrunk(Trunk trunkObj)
        {
            InsertOperationOutput<TrunkDetail> insertOperationOutput = new InsertOperationOutput<TrunkDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int trunkId = -1;

            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            bool inserted = dataManager.AddTrunk(trunkObj, out trunkId);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;

                if (trunkObj.LinkedToTrunkId != null)
                {
                    int linkedToTrunkId = (int)trunkObj.LinkedToTrunkId;

                    dataManager.UnlinkTrunk(linkedToTrunkId);
                    dataManager.LinkTrunks(trunkId, linkedToTrunkId);
                }

                insertOperationOutput.InsertedObject = dataManager.GetTrunkById(trunkId);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<TrunkDetail> UpdateTrunk(Trunk trunkObj)
        {
            UpdateOperationOutput<TrunkDetail> updateOperationOutput = new UpdateOperationOutput<TrunkDetail>();
            
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();

            bool updated = dataManager.UpdateTrunk(trunkObj);

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;

                dataManager.UnlinkTrunk(trunkObj.TrunkId);

                if (trunkObj.LinkedToTrunkId != null)
                {
                    int linkedToTrunkId = (int)trunkObj.LinkedToTrunkId;
                    dataManager.UnlinkTrunk(linkedToTrunkId);

                    dataManager.LinkTrunks(trunkObj.TrunkId, linkedToTrunkId);
                }

                updateOperationOutput.UpdatedObject = dataManager.GetTrunkById(trunkObj.TrunkId);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteTrunk(int trunkId, int? linkedToTrunkId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();

            bool deleted = dataManager.DeleteTrunk(trunkId);

            if (linkedToTrunkId != null)
            {
                int id = (int)linkedToTrunkId;
                dataManager.UnlinkTrunk(id);
            }

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
