using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Extensions;
using Vanrise.BusinessProcess.Web.ModelMappers;
using Vanrise.BusinessProcess.Web.Models;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    public partial  class BusinessProcessController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public BPDefinition GetDefinition(int ID)
        {
            BPClient manager = new BPClient();
            return manager.GetDefinition(ID);
        }

        [HttpPost]
        public CreateProcessOutput CreateNewProcess(CreateProcessInput createProcessInput)
        {
            BPClient manager = new BPClient();
            return manager.CreateNewProcess(createProcessInput);
        }


        [HttpGet]
        public IEnumerable<BPInstanceModel> GetOpenedInstances()
        {
            BPClient manager = new BPClient();
            return BPMappers.MapTMapInstances(manager.GetOpenedInstances());
        }

        [HttpGet]
        public List<BPDefinition> GetFilteredDefinitions(int fromRow, int toRow, string title)
        {
            BPClient manager = new BPClient();
            return manager.GetFilteredDefinitions(fromRow, toRow, title);
        }

        [HttpGet]
        public List<BPDefinition> GetDefinitions()
        {
            BPClient manager = new BPClient();
            return manager.GetDefinitions();
        }


        [HttpGet]
        public List<EnumModel> GetStatusList()
        {
            var lst = new List<EnumModel>();
            foreach (var val in Enum.GetValues(typeof(BPInstanceStatus)))
            {
                EnumModel item = new EnumModel
                {
                    Value = (int)val,
                    Description = ((BPInstanceStatus)val).ToString()
                };
                lst.Add(item);
            }
            return lst;
        }

        [HttpGet]
        public List<EnumModel> GetTrackingSeverity()
        {
            var lst = new List<EnumModel>();
            foreach (var val in Enum.GetValues(typeof(BPTrackingSeverity)))
            {
                EnumModel item = new EnumModel
                {
                    Value = (int)val,
                    Description = ((BPTrackingSeverity)val).ToString()
                };
                lst.Add(item);
            }
            return lst;
        }

        [HttpGet]
        public List<SchedulerTask> GetWorkflowTasksByDefinitionId(int bpDefinitionId)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            List<SchedulerTask> workflowTasks = manager.GetTasksbyActionType(1);

            List<SchedulerTask> filteredList = new List<SchedulerTask>();

            foreach (SchedulerTask task in workflowTasks)
            {
                if (((WFSchedulerTaskAction)task.TaskAction).BPDefinitionID == bpDefinitionId)
                    filteredList.Add(task);
            }

            return filteredList;
        }

        [HttpPost]
        public IEnumerable<BPInstanceModel> GetFilteredBProcess(GetFilteredBProcessInput param)
        {
            param.FromRow = param.FromRow - 1;
            BPClient manager = new BPClient();
            IEnumerable<BPInstanceModel> rows = BPMappers. MapTMapInstances(manager.GetFilteredInstances(param.DefinitionsId,param.InstanceStatus, param.DateFrom.HasValue ? param.DateFrom.Value : DateTime.Now.AddHours(-1), param.DateTo.HasValue ? param.DateTo.Value : DateTime.Now));
            rows = rows.Skip(param.FromRow ).Take(param.ToRow - param.FromRow);
            return rows;
        }


        [HttpPost]
        public GetTrackingsByInstanceIdOutput GetTrackingsByInstanceId(GetTrackingsByInstanceIdInput param)
        {
            param.FromRow = param.FromRow - 1;
            BPClient manager = new BPClient();
            IEnumerable<BPTrackingMessageModel> rows = BPMappers.MapTrackingMessages(manager.GetTrackingsByInstanceId(param.ProcessInstanceID,param.LastTrackingId));
            rows = rows.Skip(param.FromRow).Take(param.ToRow - param.FromRow);
            return new GetTrackingsByInstanceIdOutput(){
                Tracking = rows,
                InstanceStatus = manager.GetInstance(param.ProcessInstanceID).Status
            };
        }

        [HttpGet]
        public BPInstanceModel GetBPInstance(int id)
        {
            
            BPClient manager = new BPClient();
            return BPMappers.MapInstance(manager.GetInstance(id));
        }

        [HttpGet]
        public IEnumerable<BPInstanceStatus> GetNonClosedStatuses()
        {
            return BPInstanceStatusAttribute.GetNonClosedStatuses();
        }

    }

    #region Argument Classes
    
    public class GetFilteredBProcessInput
    {
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public List<int> DefinitionsId { get; set; }

        public List<BPInstanceStatus> InstanceStatus { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }

    public class GetTrackingsByInstanceIdInput
    {
        public long ProcessInstanceID { get; set; }
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public long LastTrackingId { get; set; }

    }

    public class GetTrackingsByInstanceIdOutput
    {
        public IEnumerable<BPTrackingMessageModel> Tracking { get; set; }

        public BPInstanceStatus InstanceStatus { get; set; }
        
    }

    #endregion

}