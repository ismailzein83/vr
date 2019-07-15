using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class UpdateContractAddressFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "CF55D771-7B81-4BEE-8A38-7AF50D390A6F";
        const string changeAddressStep = "6CCE155B-172B-4E10-83DE-D07B113DA1A9";
        const string directoryInquiryStep = "239FB2EE-BA09-4F13-A170-9E77CAF8109D";
        const string paymentStep = "B904469E-5B4C-4699-BFBF-729691E1A26C";
        const string submitToOMStep = "BB2D61D2-96B0-496D-9843-AF97FDDFE82F";
        const string completedStep = "4AFB7452-F5FF-4CB7-8AE3-79801A6DEB7F";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = changeAddressStep; break;
                case changeAddressStep: nextStepId = directoryInquiryStep; break;
                case directoryInquiryStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = submitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
