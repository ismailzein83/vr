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
        const string paymentStep = "0AD7DC9D-2F75-449D-A6CD-6D78F4EFEDF7";

        const string printContractStep = "545FFAEC-162A-43A9-AAB6-67C68D9C8BB9";
        const string credentialsStep = "21FDEC10-505B-4A6A-BA7D-634B387BBDCF";

        const string printCredentialsStep = "B6A7D392-3F98-434E-98C1-C8FDE914186F";
        const string attachmentStep = "30B4782C-18AA-494C-A1D3-F4DBD25A840B";
        const string waitingListStep = "ED5E126A-3336-47E6-9138-2788FF96B78A";
        const string reservePhoneNumberStep = "004A1891-960D-4D0B-9995-BE3DF680D2B8";
        const string technicalStep = "4A64F6DD-F60A-4D7D-ABB8-CC1AE8D501F6";

        const string completedStep = "2B0CDA46-E3DF-449B-A4A1-C677781CF4B9";

        public string GetNextStep(string id, string currentStepId , bool isWaitingList = false, bool isNetworkTeam = false)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = decisionIdStep; break;
                case decisionIdStep: nextStepId = printTemplateStep; break;
                case printTemplateStep: nextStepId = nearByNumberStep; break;
                case nearByNumberStep: nextStepId = freeTechnicalStep; break;
                //case freeTechnicalStep: nextStepId = canReserve(id)? addressStep : networkTeamStep; break;
                case freeTechnicalStep:

                    if (!isWaitingList && !isNetworkTeam) nextStepId = addressStep;
                    else if (isWaitingList) nextStepId = waitingListStep;
                    else nextStepId = networkTeamStep;
                    break;

                case waitingListStep: nextStepId = freeTechnicalStep; break;
                //case networkTeamStep: nextStepId = addressStep; break;
                case reservePhoneNumberStep: nextStepId = addressStep; break;
                case addressStep: nextStepId = servicesStep; break;
                case servicesStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = printContractStep; break;
                case printContractStep: nextStepId = credentialsStep; break;
                case credentialsStep: nextStepId = printCredentialsStep; break;
                case printCredentialsStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;

            }
            return nextStepId;
        }

        public bool canReserve(string id)
        {
            bool stCanReserve = false;

            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGSHDSL");
                esq.AddColumn("Id");
                esq.AddColumn("StNearbyNumber");
                // Creation of the first filter instance.
                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
                // Adding created filters to query collection. 
                esq.Filters.Add(esqFirstFilter);
                // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var phoneNumber = entities[0].GetColumnValue("StNearbyNumber");
                    stCanReserve = Boolean.Parse(new InventoryManager().GSHDSLGetTechnicalReservation(phoneNumber.ToString()).CanReserve);
                }
            }

            return stCanReserve;

        }

    }
}
