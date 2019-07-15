using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class OtherChargesFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "AF09326C-2080-427C-8CDC-8C6A1537ABC8";
        const string chargesStep = "CBE1B07E-212F-4CAF-9BF6-7F3D8BC31DAA";
        const string completedStep = "FB02B561-E247-4003-8861-C32688E4A2C0";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = chargesStep; break;
                case chargesStep: nextStepId = completedStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
