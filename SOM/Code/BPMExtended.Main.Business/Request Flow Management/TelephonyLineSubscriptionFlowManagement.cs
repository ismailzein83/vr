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
   
    public class TelephonyLineSubscriptionFlowManagement
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion
        const string welcomeStep = "26A7BD18-2E80-4901-9746-8061C7FF569D";
        const string nearByNumberStep = "7C14E38A-F5CC-46DD-A94E-B49871B819E8";
        const string printTemplateStep = "6895F711-7E76-4043-8777-9267D7CD27EC";
        const string findFreeTechnicalStep = "EE7C2188-77A0-406C-BAA5-E29925509AE8";

        const string waitingListStep = "E054EE92-661A-42D2-B0F6-9D78B12331B7";
        const string networkTeamStep = "73F41594-A8DB-4967-ACB0-3B8B8E0694A8";
        const string reservePhoneNumberStep = "64BD8A64-E4E7-4F9C-8497-A466F2B35350";
        const string subscriptionAddressStep = "74FA6D04-804D-45D7-B3E3-887A98450F13";

        const string servicesStep = "EEC0CF10-F7E2-45AD-953F-A35C1DAA4B27";
        const string foreignerStep = "351B5CF9-AB1E-47E2-BC93-B8B4F101D047";
        const string contractOnHoldStep = "3B1E25EA-6751-4BDC-99F8-0CFAC2B9A25E";
        const string paymentStep = "6FD4F52A-310D-4E91-999D-F2D6FEC19AB0";
        const string printContractStep = "1B93BB41-AA73-4430-9DB0-424243E1EE11";

        const string technicalStep = "5AC289E9-1489-423E-B6E0-32479C2A127B";
        const string submitToOMStep = "EB12BB10-966F-43FB-BD4A-A815CB168DBE";

        //string completedStep = "EBACDCBA-0DB9-4582-999B-6317DA0094A7";

        public string GetNextStep(string id, string currentStepId , bool isWaitingList = false, bool isNetworkTeam = false)
        {

            string nextStepId = "";
            switch (currentStepId)
            {   
                //welcome
                case welcomeStep: nextStepId = nearByNumberStep; break;
                case nearByNumberStep: nextStepId = printTemplateStep; break;
                case printTemplateStep: nextStepId = findFreeTechnicalStep; break;
                case findFreeTechnicalStep:
                    if (!isWaitingList && !isNetworkTeam) nextStepId = subscriptionAddressStep;
                    else if (isWaitingList) nextStepId = waitingListStep;
                    else nextStepId = technicalStep;
                    break;
                case waitingListStep: nextStepId = findFreeTechnicalStep; break;
                case networkTeamStep: nextStepId = reservePhoneNumberStep; break;
                case reservePhoneNumberStep: nextStepId = subscriptionAddressStep; break;
                case subscriptionAddressStep: nextStepId = servicesStep; break;
                case servicesStep:nextStepId = this.IsContactForeigner(id) ? foreignerStep : contractOnHoldStep; break;  //nextStepId = contractOnHoldStep; break;-
                case foreignerStep: nextStepId = contractOnHoldStep; break;
                case contractOnHoldStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = printContractStep; break;
                case printContractStep: nextStepId = submitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
        public bool IsContactForeigner(string Id)
        {
            bool isForeigner = false;
            EntitySchemaQuery esq;
            EntityCollection entities;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("Id");
            var contactcol = esq.AddColumn("StContact.Id");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", Id));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                Guid ContactId = entities[0].GetTypedColumnValue<Guid>(contactcol.Name);
                CRMCustomerManager manager = new CRMCustomerManager();
                isForeigner = manager.IsContactForeigner(ContactId);
            }
            return isForeigner;
        }
    }
}
