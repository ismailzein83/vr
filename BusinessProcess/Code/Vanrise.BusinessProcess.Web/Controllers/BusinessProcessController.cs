using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
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
    public class BusinessProcessController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public BPDefinition GetDefinition(int id)
        {
            BPClient manager = new BPClient();
            return manager.GetDefinition(id);
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
        public List<BPDefinition> GetFilteredDefinitions(string title)
        {
            BPClient manager = new BPClient();
            return manager.GetFilteredDefinitions(title);
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


        [HttpGet]
        public List<SchedulerTaskModel> GetWorkflowTasksByDefinitionIds()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            List<SchedulerTask> workflowTasks = manager.GetTasksbyActionType(1);

            List<SchedulerTask> filteredList = new List<SchedulerTask>();

            foreach (SchedulerTask task in workflowTasks)
            {
                    filteredList.Add(task);
            }

            return BPMappers.MapTMapSchedulerTasks(filteredList);
        }



        [HttpPost]
        public Object GetFilteredBProcess(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        {
            BPClient manager = new BPClient();
            return GetWebResponse(input, manager.GetFilteredInstances(input));
        }


        [HttpPost]
        public Object GetFilteredTrackings(Vanrise.Entities.DataRetrievalInput<TrackingQuery> input)
        {
            BPClient manager = new BPClient();
            return GetWebResponse(input,manager.GetTrackingsByInstanceId(input));
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
    

    public class GetTrackingsByInstanceIdOutput
    {
        public IEnumerable<BPTrackingMessageModel> Tracking { get; set; }

        public BPInstanceStatus InstanceStatus { get; set; }
        
    }

    #endregion

}