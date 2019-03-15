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
        //const string switchTeamStep = "9ADF25A4-C8CC-4383-8D82-0E37AAF8E895";
        const string technicalStep = "DFB53DD9-36C1-461B-AF90-080E3B0B5B60";
        const string completedStep = "4467A681-9D21-4577-8079-DA49D1B61616";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = manageContractsStep; break;
                case manageContractsStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;
            }
            return nextStepId;
        }


    }
}
