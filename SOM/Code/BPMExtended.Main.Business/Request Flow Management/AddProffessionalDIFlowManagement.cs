using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class AddProffessionalDIFlowManagement
    {
        const string welcomeStep = "58875F71-B8D2-4FA2-B0AD-3788D793D5EE";
        const string professionStep = "0E411A03-C47D-4641-957E-87ED88554146";
        const string paymentStep = "76B2379E-18F0-4BB1-B3DC-4CCAF5170B03";
        const string submitToOMStep = "19CCCD29-02BF-4C58-A8DB-2519A83B6B2C";
        const string completedStep = "AA7645DC-01E0-4A72-ACC0-09FA4D93E5EB";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = professionStep; break;
                case professionStep: nextStepId = new CommonManager().IsRequestAdvantageous(new CommonManager().GetEntityNameByRequestId(id), id) ? submitToOMStep : paymentStep; break; 
                case paymentStep: nextStepId = submitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
