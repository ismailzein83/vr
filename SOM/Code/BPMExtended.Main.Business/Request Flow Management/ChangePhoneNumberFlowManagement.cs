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
        const string paymentStep = "AD9AE38B-740D-4122-88A3-BFCCD5DBF288";
        const string validatePaymentStep = "3207851A-4BA6-463C-9D38-77D8B8C60F11";

        const string testToneStep = "7E97071D-2AF6-4FD9-8379-D069C0DD86C5";

        const string mdfOldStep = "FA22EF11-6128-4EF3-B0DD-CEEC70692A93";
        const string mdfNewStep = "EB654A7F-FC0A-4A82-8477-D95F2B446B06";

        const string completedStep = "A359D1DA-9970-4C97-A669-8F30E7B31550";

        public string GetNextStep(string id, string currentStepId, bool isPaymentTypeCash)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = printStep; break;
                case printStep: nextStepId = lineDescriptionStep; break;
                case lineDescriptionStep: nextStepId = choosePhoneNumberStep; break;
                case choosePhoneNumberStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = isPaymentTypeCash ? validatePaymentStep :testToneStep ; break;
                case validatePaymentStep: nextStepId = testToneStep; break;
                case testToneStep: nextStepId = mdfOldStep; break;
                case mdfOldStep: nextStepId = mdfNewStep; break;
                case mdfNewStep: nextStepId = completedStep; break;

            }
            return nextStepId;
        }


    }
}
