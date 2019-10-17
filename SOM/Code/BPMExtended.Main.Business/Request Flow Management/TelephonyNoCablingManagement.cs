using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class TelephonyNoCablingManagement
    {
        const string welcomeStep = "a75fb3a2-d719-47a7-9b34-17ca618cd174";
        const string choosePhoneNumberStep = "200c4a8c-9926-401b-9a54-16cf70cfd3e2";
        const string addressStep = "baf6e554-ba58-4ac3-afc5-d1e3c13355ca";
        const string ratePlanStep = "e80afab5-9438-43bd-905c-169ba441b265";
        const string createContractOnHoldStep = "69f042a1-23a6-4d7f-ad53-8c95b5d8ed6e";
        const string paymentStep = "a6f860f7-f3e1-4528-8746-fe4a233e25bc";
        const string printStep = "49dabdc5-70d7-45f3-a2e7-acd507abbcb4";
        const string attachmentstep = "aa241f92-b894-416a-b119-1ce27fcfe87e";
        const string technicalStep= "3257c859-885b-410e-a3a2-fe99e0723a62";
        const string submittedtoomStep = "75159e2f-bc0b-4765-9771-5d1c3fc153ab";
        const string completedStep = "ee43e28d-aa12-41a3-8c97-626dcaad95b0";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = choosePhoneNumberStep; break;
                case choosePhoneNumberStep: nextStepId = technicalStep; break;
                case addressStep: nextStepId = ratePlanStep; break;
                case ratePlanStep: nextStepId = createContractOnHoldStep; break;
                case createContractOnHoldStep: nextStepId = new CommonManager().IsRequestAdvantageous(new CommonManager().GetEntityNameByRequestId(id), id) ? printStep: paymentStep; break;
                case paymentStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentstep; break;
                case attachmentstep: nextStepId = submittedtoomStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }

    }
}
