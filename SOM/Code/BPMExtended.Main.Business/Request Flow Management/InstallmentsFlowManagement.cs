using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business.Request_Flow_Management
{
   public class InstallmentsFlowManagement
    {
        const string startingProcess = "DB262400-C7ED-4C0F-9578-7450AF5EAA0F";
        const string installmentsData = "A7079344-9FB4-4989-A574-419DB6B02106";
        const string paymentStep = "67F6692E-D79B-4031-BBFB-B9D149790AA6";
        const string technicalStep = "D5CAA3E5-7879-4679-A30D-7F325825D5AF";
        const string endProcess = "4D3FD81C-6F7B-4E6C-ABAC-71613276A924";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = installmentsData; break;
                case installmentsData: nextStepId = technicalStep; break;
                case paymentStep: nextStepId = endProcess; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }

    }
}
