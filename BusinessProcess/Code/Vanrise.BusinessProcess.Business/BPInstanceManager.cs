using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace Vanrise.BusinessProcess.Business
{
    public class BPInstanceManager
    {
        #region public methods
        public List<BPInstanceDetail> GetBeforeId(BPInstanceBeforeIdInput input)
        {
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();

            List<BPInstance> bpInstances = dataManager.GetBeforeId(input);
            List<BPInstanceDetail> bpInstanceDetails = new List<BPInstanceDetail>();
            foreach (BPInstance bpInstance in bpInstances)
            {
                bpInstanceDetails.Add(BPInstanceDetailMapper(bpInstance));
            }
            return bpInstanceDetails;
        }

        public BPInstanceUpdateOutput GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, string entityId)
        {
            BPInstanceUpdateOutput bpInstanceUpdateOutput = new BPInstanceUpdateOutput();

            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();

            List<BPInstance> bpInstances = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, definitionsId, parentId, entityId);
            List<BPInstanceDetail> bpInstanceDetails = new List<BPInstanceDetail>();
            foreach (BPInstance bpInstance in bpInstances)
            {
                bpInstanceDetails.Add(BPInstanceDetailMapper(bpInstance));
            }

            bpInstanceUpdateOutput.ListBPInstanceDetails = bpInstanceDetails;
            bpInstanceUpdateOutput.MaxTimeStamp = maxTimeStamp;
            return bpInstanceUpdateOutput;
        }

        public Vanrise.Entities.IDataRetrievalResult<BPInstanceDetail> GetFilteredBPInstances(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new BPInstanceRequestHandler());
        }

        public BPInstance GetBPInstance(long bpInstanceId)
        {
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            return dataManager.GetBPInstance(bpInstanceId);
        }

        public CreateProcessOutput CreateNewProcess(CreateProcessInput createProcessInput)
        {
            if (createProcessInput == null)
                throw new ArgumentNullException("createProcessInput");
            if (createProcessInput.InputArguments == null)
                throw new ArgumentNullException("createProcessInput.InputArguments");
            if (createProcessInput.InputArguments.UserId <= 0)
                throw new ArgumentException("createProcessInput.InputArguments.UserId");

            BPDefinitionManager bpDefinitionManager = new BPDefinitionManager();
            BPDefinition processDefinition = bpDefinitionManager.GetDefinition(createProcessInput.InputArguments.ProcessName);
            if (processDefinition == null)
                throw new Exception(String.Format("No Process Definition found match with input argument '{0}'", createProcessInput.InputArguments.GetType()));

            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            string processTitle = createProcessInput.InputArguments.GetTitle();
            if (processTitle != null)
                processTitle = processTitle.Replace("#BPDefinitionTitle#", processDefinition.Title);
            long processInstanceId = dataManager.InsertInstance(processTitle, createProcessInput.ParentProcessID, createProcessInput.CompletionNotifier, processDefinition.BPDefinitionID, createProcessInput.InputArguments, BPInstanceStatus.New, createProcessInput.InputArguments.UserId, createProcessInput.InputArguments.EntityId);
            IBPTrackingDataManager dataManagerTracking = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            dataManagerTracking.Insert(new BPTrackingMessage
            {
                ProcessInstanceId = processInstanceId,
                ParentProcessId = createProcessInput.ParentProcessID,
                TrackingMessage = String.Format("Process Created: {0}", processTitle),
                Severity = LogEntryType.Information,
                EventTime = DateTime.Now
            });
            CreateProcessOutput output = new CreateProcessOutput
            {
                ProcessInstanceId = processInstanceId,
                Result = CreateProcessResult.Succeeded
            };
            return output;
        }


        #endregion

        #region mapper
        private BPInstanceDetail BPInstanceDetailMapper(BPInstance bpInstance)
        {
            if (bpInstance == null)
                return null;
            string bpDefinitionTitle = null;
            string userName = new UserManager().GetUserName(bpInstance.InitiatorUserId);
            var bpDefinition = new BPDefinitionManager().GetBPDefinition(bpInstance.DefinitionID);
            if (bpDefinition != null)
                bpDefinitionTitle = bpDefinition.Title;
            return new BPInstanceDetail()
            {
                Entity = bpInstance,
                DefinitionTitle = bpDefinitionTitle,
                UserName = userName
            };
        }
        #endregion


        #region Private Classes

        private class BPInstanceRequestHandler : BigDataRequestHandler<BPInstanceQuery, BPInstance, BPInstanceDetail>
        {
            public override BPInstanceDetail EntityDetailMapper(BPInstance entity)
            {
                BPInstanceManager manager = new BPInstanceManager();
                return manager.BPInstanceDetailMapper(entity);
            }

            public override IEnumerable<BPInstance> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
            {
                IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
                return dataManager.GetAllBPInstances(input.Query);
            }
        }

        #endregion
    }
}
