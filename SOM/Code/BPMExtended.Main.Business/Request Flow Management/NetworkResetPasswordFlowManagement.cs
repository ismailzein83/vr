using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class NetworkResetPasswordFlowManagement
    {

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "0126F306-69C3-4954-B195-8B13091932DA";
        //const string resetStep = "6E18BF0A-0AF4-409B-9594-C1C12AEE1914";
        const string payment = "41B3AC84-3AE8-4D0F-A352-D68DD53812A6";
        const string technicalStep = "D3D55306-886B-4087-8DEC-8249D28EF7CB";
        const string submitToOM = "6B5A8026-A12D-4844-8302-84ACFCFBBFBE";
        const string completedStep = "9730D12A-23FB-45D9-AB4E-126B1028BE91";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = payment; break;
               // case resetStep: nextStepId = payment; break;
                case payment: nextStepId = technicalStep; break;
                case technicalStep: nextStepId = submitToOM; break;
                case submitToOM: nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
