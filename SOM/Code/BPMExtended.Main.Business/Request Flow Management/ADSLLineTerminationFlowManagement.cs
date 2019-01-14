﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrasoft.Core;
using Terrasoft.Core.Configuration;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;
using System.Web;

namespace BPMExtended.Main.Business
{
    public class ADSLLineTerminationFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        string welcomeStep = "B8721EA5-0A7E-43BC-8E8D-8C186537FD73";
        string printStep = "0705ADD3-50F2-42B4-8247-4B67E85E166C";
        string ReasonStep = "DA251EA7-47C1-42AD-B4EC-A131EDD9587A";
        string paymentStep = "A0297B0C-53AD-43A9-8568-160317FE99B8";
        string attachmentStep = "FB8D7425-046F-4FCC-8983-F9C5A9336DCE";
        string TechnicalStep = "56D55B17-8962-4D80-9564-361AD127E6B5";
        string completedStep = "639E4A4C-1752-42E3-83CD-85674B6E9903";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId.ToLower())
            {
                ///welcome step
                case "b8721ea5-0a7e-43bc-8e8d-8c186537fd73": nextStepId = printStep; break;
                ///print step
                case "0705add3-50f2-42b4-8247-4b67e85e166c": nextStepId = ReasonStep; break;

                case "da251ea7-47c1-42ad-b4ec-a131edd9587a": nextStepId = IsAdavatageous(id)?attachmentStep : paymentStep; break;

                ///payment step
                case "a0297b0c-53ad-43a9-8568-160317fe99b8": nextStepId = attachmentStep; break;

                //attachment
                case "fb8d7425-046f-4fcc-8983-f9c5a9336dce": nextStepId = TechnicalStep; break;
              
                /// Technical Step
                case "56d55b17-8962-4d80-9564-361ad127e6b5": nextStepId = completedStep; break;
            }
            return nextStepId;
        }
          public bool IsAdavatageous(string id)
          {
              bool isAdavatageous = false;
              if (id != "")
              {
                  //get request from bpm
                  Guid idd = new Guid(id.ToUpper());
                  var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineTermination");
                  esq.AddColumn("Id");
                  esq.AddColumn("StAdvantageous");
                  // Creation of the first filter instance.
                  var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
                  // Adding created filters to query collection. 
                  esq.Filters.Add(esqFirstFilter);
                  // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                  var entities = esq.GetEntityCollection(BPM_UserConnection);
                  if (entities.Count > 0)
                  {
                      return (bool)entities[0].GetColumnValue("StAdvantageous");
                  }
              }
              return isAdavatageous;
          }
    }
}
