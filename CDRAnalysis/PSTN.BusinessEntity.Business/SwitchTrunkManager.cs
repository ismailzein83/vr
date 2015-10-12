﻿using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class SwitchTrunkManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SwitchTrunkDetail> GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkQuery> input)
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSwitchTrunks(input));
        }

        public SwitchTrunkDetail GetSwitchTrunkByID(int trunkID)
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return dataManager.GetSwitchTrunkByID(trunkID);
        }

        public SwitchTrunkInfo GetSwitchTrunkBySymbol(string symbol)
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return dataManager.GetSwitchTrunkBySymbol(symbol);
        }

        public List<SwitchTrunkInfo> GetTrunksBySwitchIds(TrunkFilter trunkFilterObj)
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return dataManager.GetTrunksBySwitchIds(trunkFilterObj);
        }

        public List<SwitchTrunkInfo> GetSwitchTrunks()
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return dataManager.GetSwitchTrunks();
        }

        public List<SwitchTrunkInfo> GetSwitchTrunksByIds(List<int> trunkIds)
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return dataManager.GetSwitchTrunksByIds(trunkIds);
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

                if (trunkObject.LinkedToTrunkID != null)
                {
                    int linkedToTrunkID = (int)trunkObject.LinkedToTrunkID;

                    dataManager.UnlinkSwitchTrunk(linkedToTrunkID);
                    dataManager.LinkSwitchTrunks(trunkID, linkedToTrunkID);
                }

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

                dataManager.UnlinkSwitchTrunk(trunkObject.ID);

                if (trunkObject.LinkedToTrunkID != null)
                {
                    int linkedToTrunkID = (int)trunkObject.LinkedToTrunkID;
                    dataManager.UnlinkSwitchTrunk(linkedToTrunkID);

                    dataManager.LinkSwitchTrunks(trunkObject.ID, linkedToTrunkID);
                }

                updateOperationOutput.UpdatedObject = dataManager.GetSwitchTrunkByID(trunkObject.ID);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteSwitchTrunk(int trunkID, int? linkedToTrunkID)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();

            bool deleted = dataManager.DeleteSwitchTrunk(trunkID);

            if (linkedToTrunkID != null)
            {
                int id = (int)linkedToTrunkID;
                dataManager.UnlinkSwitchTrunk(id);
            }

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
