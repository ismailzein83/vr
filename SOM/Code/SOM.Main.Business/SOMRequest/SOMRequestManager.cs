using SOM.Main.Data;
using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace SOM.Main.Business
{
    public class SOMRequestManager
    {
        static Vanrise.BusinessProcess.Business.BPInstanceManager s_bpInstanceManager = new Vanrise.BusinessProcess.Business.BPInstanceManager();
        static ISOMRequestDataManager s_dataManager = MainDataManagerFactory.GetDataManager<ISOMRequestDataManager>();

        #region Public Methods

        public CreateSOMRequestOutput CreateSOMRequest(CreateSOMRequestInput input)
        {
            input.ThrowIfNull("input");
            input.Settings.ThrowIfNull("input.Settings");
            input.Settings.ExtendedSettings.ThrowIfNull("input.Settings.ExtendedSettings");
            Guid requestTypeId = input.Settings.ExtendedSettings.ConfigId;
            string serializedSettings = Serializer.Serialize(input.Settings);
            long requestId;
            s_dataManager.AddRequest(requestTypeId, input.EntityId, serializedSettings, out requestId);
            var createBPInputArgContext = new SOMRequestConvertToBPInputArgumentContext();
            BaseSOMRequestBPInputArg bpInputArg = input.Settings.ExtendedSettings.ConvertToBPInputArgument(createBPInputArgContext);
            bpInputArg.ThrowIfNull("bpInputArg");
            bpInputArg.EntityId = input.EntityId;
            bpInputArg.UserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            bpInputArg.SOMRequestId = requestId;
            bpInputArg.SOMRequestTypeId = requestTypeId;
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = bpInputArg
            };
            var createProcessOutput = s_bpInstanceManager.CreateNewProcess(createProcessInput);
            s_dataManager.UpdateRequestProcessInstanceId(requestId, createProcessOutput.ProcessInstanceId);
            return new CreateSOMRequestOutput
            {
                SOMRequestId = createProcessOutput.ProcessInstanceId
            };
        }

        public Vanrise.Entities.IDataRetrievalResult<SOMRequestDetail> GetFilteredSOMRequests(Vanrise.Entities.DataRetrievalInput<SOMRequestQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SOMRequestRequestHandler());
        }

        #endregion

        #region Private Classes

        private class SOMRequestConvertToBPInputArgumentContext : ISOMRequestConvertToBPInputArgumentContext
        {

        }

        private class SOMRequestRequestHandler : BigDataRequestHandler<SOMRequestQuery, SOMRequestDetail, SOMRequestDetail>
        {
            static BPInstanceManager s_instanceManager = new BPInstanceManager();
            public override SOMRequestDetail EntityDetailMapper(SOMRequestDetail entity)
            {
                return entity;
            }

            public override IEnumerable<SOMRequestDetail> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SOMRequestQuery> input)
            {
                SOMRequestQuery somRequestQuery = input.Query;
                var bpInstanceQuery = new BPInstanceQuery
                {
                    DateFrom = somRequestQuery.FromTime,
                    DateTo = somRequestQuery.ToTime,
                    EntityId = somRequestQuery.EntityId
                };
                List<BPInstance> bpInstances = s_bpInstanceManager.GetAllFilteredBPInstances(bpInstanceQuery);
                if (bpInstances != null)
                {
                    List<SOMRequestDetail> somRequests = new List<SOMRequestDetail>();
                    foreach (var bpInstance in bpInstances)
                    {
                        var somRequest = new SOMRequestDetail
                        {
                            SOMRequestId = bpInstance.ProcessInstanceID,
                            ProcessInstanceId = bpInstance.ProcessInstanceID,
                            Title = bpInstance.Title,
                            RequestTypeId = bpInstance.InputArgument.CastWithValidate<BaseSOMRequestBPInputArg>("bpInstance.InputArgument", bpInstance.ProcessInstanceID).SOMRequestTypeId,
                            CreatedTime = bpInstance.CreatedTime,
                            EntityId = bpInstance.EntityId,
                            Status = MapBPInstanceStatus(bpInstance.Status)
                        };
                        somRequest.StatusDescription = Utilities.GetEnumDescription(somRequest.Status);
                        somRequests.Add(somRequest);
                    }
                    return somRequests;
                }
                else
                {
                    return null;
                }
            }

            private SOMRequestStatus MapBPInstanceStatus(BPInstanceStatus bpInstanceStatus)
            {
                switch (bpInstanceStatus)
                {
                    case BPInstanceStatus.New:
                    case BPInstanceStatus.Postponed:
                        return SOMRequestStatus.New;
                    case BPInstanceStatus.Waiting:
                        return SOMRequestStatus.Waiting;
                    case BPInstanceStatus.Running:
                        return SOMRequestStatus.Running;
                    case BPInstanceStatus.Completed:
                        return SOMRequestStatus.Completed;
                    case BPInstanceStatus.Aborted:
                    case BPInstanceStatus.Suspended:
                    case BPInstanceStatus.Terminated:
                        return SOMRequestStatus.Aborted;
                    default: throw new NotSupportedException(String.Format("bpInstanceStatus '{0}'", bpInstanceStatus.ToString()));
                }
            }
        }
        
        #endregion
    }
}