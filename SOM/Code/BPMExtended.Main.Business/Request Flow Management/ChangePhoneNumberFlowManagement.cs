using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ChangePhoneNumberFlowManagement
    {
        const string welcomeStep = "07878869-D069-4185-94FA-A27DE44571CC";
        const string printStep = "36C51A0F-C78B-4EEA-9F4B-B24F80C6C53B";
        const string lineDescriptionStep = "AA871142-C408-48CF-A06A-DE0ED285F166";

        const string choosePhoneNumberStep = "94C9BF32-6459-473D-A21E-D940BCAE74F9";
        const string changeRatePlanStep = "00807DD5-9F6C-4952-8DC2-F7DD1DCE0CF0";
        const string paymentStep = "AD9AE38B-740D-4122-88A3-BFCCD5DBF288";
        //const string validatePaymentStep = "3207851A-4BA6-463C-9D38-77D8B8C60F11";
        const string attachmentStep = "CBA12E12-AEE8-4053-877A-88234A877FE1";
        const string submitToOMStep = "04034AE2-4078-4722-A223-B400C13F89A3";
        const string technicalStep = "2B8B0E7C-9925-4ABE-A9E7-145E96F77982";

        public string GetNextStep(string id, string currentStepId,bool isSubTypeChanged)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = choosePhoneNumberStep; break;
                case choosePhoneNumberStep: nextStepId = isSubTypeChanged?changeRatePlanStep : printStep; break;
                case changeRatePlanStep: nextStepId = printStep; break;
                case printStep: nextStepId = new CommonManager().IsRequestAdvantageous(new CommonManager().GetEntityNameByRequestId(id), id) ? attachmentStep : paymentStep; break;
                case paymentStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }


    }
}
