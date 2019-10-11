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
    public class GSHDSLFlowManagement
    {

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }


        const string welcomeStep = "57A68339-09AB-4AC2-BA95-0F0217B76720";
        const string decisionIdStep = "AF321326-B615-4D6E-80C4-471506C14ED0";
        const string printTemplateStep = "48EB7E35-A501-442A-B628-57472BF330D1";

        const string nearByNumberStep = "821E7FF9-3303-4F89-96FF-688CEA98D8FB";
        const string freeTechnicalStep = "0000461F-10D0-4476-AC56-33C741460DFC";
        const string networkTeamStep = "04C32E1E-A212-4B30-AA00-C5072DD8DD42";

        const string addressStep = "25C17DAA-A88C-49EA-80EA-E592A9062C2C";

        const string servicesStep = "4E7683D1-6A0F-47FB-9310-5438328C3659";
        const string createContractStep = "75D1C37F-A470-4AF8-828D-5AB590FB27D9";
        const string paymentStep = "0AD7DC9D-2F75-449D-A6CD-6D78F4EFEDF7";

        const string printContractStep = "545FFAEC-162A-43A9-AAB6-67C68D9C8BB9";
        const string credentialsStep = "21FDEC10-505B-4A6A-BA7D-634B387BBDCF";

        const string printCredentialsStep = "B6A7D392-3F98-434E-98C1-C8FDE914186F";
        const string attachmentStep = "0DA92B1F-CB6C-4DB1-95E2-B40A36331315";
        const string waitingListStep = "ED5E126A-3336-47E6-9138-2788FF96B78A";
        const string reservePhoneNumberStep = "004A1891-960D-4D0B-9995-BE3DF680D2B8";
        const string technicalStep = "4A64F6DD-F60A-4D7D-ABB8-CC1AE8D501F6";
        const string submitToOMStep = "DEF0E171-327E-4E4C-9A62-CE0FA825E060";

        const string completedStep = "2B0CDA46-E3DF-449B-A4A1-C677781CF4B9";

        public string GetNextStep(string id, string currentStepId , bool isWaitingList = false, bool isNetworkTeam = false)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = nearByNumberStep; break;
                case nearByNumberStep: nextStepId = printTemplateStep; break;
                case printTemplateStep: nextStepId = freeTechnicalStep; break;
                //case freeTechnicalStep: nextStepId = canReserve(id)? addressStep : networkTeamStep; break;
                case freeTechnicalStep:
                    if (isWaitingList) nextStepId = waitingListStep;
                    else if (isNetworkTeam) nextStepId = technicalStep;
                    else nextStepId = addressStep;
                    break;
                case waitingListStep: nextStepId = freeTechnicalStep; break;
                case addressStep: nextStepId = credentialsStep; break;
                case credentialsStep: nextStepId = printCredentialsStep; break;
                case printCredentialsStep: nextStepId = servicesStep; break;
                case servicesStep: nextStepId = createContractStep; break;
                case createContractStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = printContractStep; break;
                case printContractStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = submitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
