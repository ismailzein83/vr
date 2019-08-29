using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class SuspensionFlowManagement
    {
        const string welcomeStep = "1ed175e4-dccc-445a-bf92-3e73d2ef0bff";
        //const string paymentStep = "4BAF697B-E9E1-417A-8531-525EA4077902";
        const string attachmentstep = "2cfcc41f-89fa-4355-a5f8-cbdee8bd47c1";
        const string submittedtoom = "373fe039-c668-4376-96a0-44d96f73c372";
        const string completedStep = "8340ad42-0c1e-4063-ba73-74b3cd917c2f";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = attachmentstep; break;
                //case paymentStep: nextStepId = attachmentstep; break;
                case attachmentstep: nextStepId = submittedtoom; break;
                case submittedtoom: nextStepId = completedStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
