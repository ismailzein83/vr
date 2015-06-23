﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Web.ModelMappers;
using Vanrise.BusinessProcess.Web.Models;

namespace Vanrise.BusinessProcess.Web.Controllers
{

    public partial  class BusinessProcessController : Vanrise.Web.Base.BaseAPIController
    {

        [HttpGet]
        public List<BPInstance> GetOpenedInstances()
        {
            BPClient manager = new BPClient();
            return manager.GetOpenedInstances();
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

        
        
        [HttpPost]
        public IEnumerable<BPInstanceModel> GetFilteredBProcess(GetFilteredBProcessInput param)
        {
            BPClient manager = new BPClient();
            IEnumerable<BPInstanceModel> rows = BPMappers.MapTMapInstances(manager.GetFilteredInstances(param.DefinitionsId,param.InstanceStatus, param.DateFrom.HasValue ? param.DateFrom.Value : DateTime.Now.AddHours(-1), param.DateTo.HasValue ? param.DateTo.Value : DateTime.Now));
            rows = rows.Skip(param.FromRow).Take(param.ToRow - param.FromRow);
            return rows;
        }


        [HttpPost]
        public IEnumerable<BPTrackingMessageModel> GetTrackingsByInstanceId(GetTrackingsByInstanceIdInput param)
        {
            BPClient manager = new BPClient();
            IEnumerable<BPTrackingMessageModel> rows = BPMappers.MapTrackingMessages(manager.GetTrackingsByInstanceId(param.ProcessInstanceID,param.TrackingSeverity,param.Message));
            rows = rows.Skip(param.FromRow).Take(param.ToRow - param.FromRow);
            return rows;
        }

    }

    #region Argument Classes
    
    public class GetFilteredBProcessInput
    {
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public List<int> DefinitionsId { get; set; }

        public List<int> InstanceStatus { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }

    public class GetTrackingsByInstanceIdInput
    {
        public long ProcessInstanceID { get; set; }
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        
        public List<int> TrackingSeverity { get; set; }

        public String Message { get; set; }

    }

    #endregion

}