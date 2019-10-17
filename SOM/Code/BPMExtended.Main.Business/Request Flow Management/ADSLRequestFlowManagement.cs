using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ADSLRequestFlowManagement
    {

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "086480CE-D251-49B1-B9EC-6BE57A6D9772";
        const string chooseTelephonyContractStep = "B0C44AC6-1E43-4209-8A4B-BCF4EB3A0E65";
        const string printStep = "8D6F4638-31FD-46DD-96AA-478C1F4495ED";
        const string addressStep = "C481B85D-FB21-4EEF-B88E-246BCE388648";
        const string DSLAMStep = "86FC7101-B189-46AD-9CDD-65B35DF892E0";

        const string waitingListStep = "3F63A582-3E54-4469-83A1-5737535DCADE";
        const string servicesStep = "5961214F-D40A-4152-837A-9A85ED34AC70";
        const string paymentStep = "F04855B9-A46F-416F-84F3-7A51B621747C";
        const string adslCredentialsStep = "B1110D25-96F3-4FED-BC62-8C30623D929E";
        const string createContractOnHold = "3C1CA00A-25CE-43AC-B178-11D353B0D1B8";

        const string printConfigurationStep = "AA09C2F9-C844-4B13-BEFA-D9D4E00571FC";
        const string attachmentStep = "518BA651-509C-4500-8FC1-C5F634925907";
        const string TechnicalStep = "6AB2B0E3-5201-4189-9838-CF4B4E4E17D9";
        const string SubmitToOMStep = "DD75B34A-B1A2-4FD6-BA98-30A44894267E";

        const string completedStep = "8CD5BDC5-2551-4767-8166-5D334D5E0FD7";

        public string GetNextStep(string id, string currentStepId,bool isWaitingList)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = chooseTelephonyContractStep; break;
                case chooseTelephonyContractStep: nextStepId = addressStep; break;
                case addressStep: nextStepId = printStep; break;
                case printStep: nextStepId = DSLAMStep; break;
                case DSLAMStep: nextStepId = isWaitingList? waitingListStep : adslCredentialsStep; break;
                case waitingListStep: nextStepId = adslCredentialsStep; break;
                case adslCredentialsStep: nextStepId = servicesStep; break;
                case servicesStep: nextStepId = createContractOnHold; break;
                case createContractOnHold: nextStepId = new CommonManager().IsRequestAdvantageous(new CommonManager().GetEntityNameByRequestId(id), id) ? printConfigurationStep: paymentStep; break;
                case paymentStep: nextStepId = printConfigurationStep; break;
                case printConfigurationStep: nextStepId = SubmitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));

            }
            return nextStepId;
        }

    }
}
