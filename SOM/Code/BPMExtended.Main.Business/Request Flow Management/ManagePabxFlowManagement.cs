using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ManagePabxFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "15EA6BF0-DFFF-4BBB-BF12-2A5299BCF592";
        const string manageContractsStep = "F7D3AA83-6BAB-4A3C-8FF2-EFCBBA43ED3F";
        const string printStep = "6E8BC6D9-91C8-4B7C-9FBE-51E2565E3852";
        const string attachmentStep = "73AFE682-6C77-4A5C-8635-881DD6FCCF28";
        const string paymentStep = "E28662F4-6DDC-45F8-8159-5D0BC5C6ECB5";
        const string submittedtoOM = "A3E3BAEA-7625-4183-B584-950B43F3BC7A";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = manageContractsStep; break;
                case manageContractsStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = submittedtoOM; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }


    }
}
