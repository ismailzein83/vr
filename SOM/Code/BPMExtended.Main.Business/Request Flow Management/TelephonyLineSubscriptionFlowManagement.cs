﻿using System;
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
        string welcomeStep = "26A7BD18-2E80-4901-9746-8061C7FF569D";
        string nearByNumberStep = "7C14E38A-F5CC-46DD-A94E-B49871B819E8";
        string printTemplateStep = "6895F711-7E76-4043-8777-9267D7CD27EC";
        string findFreeTechnicalStep = "EE7C2188-77A0-406C-BAA5-E29925509AE8";

        string waitingListStep = "E054EE92-661A-42D2-B0F6-9D78B12331B7";
        string networkTeamStep = "73F41594-A8DB-4967-ACB0-3B8B8E0694A8";
        string reservePhoneNumberStep = "64BD8A64-E4E7-4F9C-8497-A466F2B35350";
        string subscriptionAddressStep = "74FA6D04-804D-45D7-B3E3-887A98450F13";

        string servicesStep = "EEC0CF10-F7E2-45AD-953F-A35C1DAA4B27";
        string foreignerStep = "351B5CF9-AB1E-47E2-BC93-B8B4F101D047";
        string contractOnHoldStep = "3B1E25EA-6751-4BDC-99F8-0CFAC2B9A25E";
        string paymentStep = "6FD4F52A-310D-4E91-999D-F2D6FEC19AB0";
        string printContractStep = "1B93BB41-AA73-4430-9DB0-424243E1EE11";

        string technicalStep = "5AC289E9-1489-423E-B6E0-32479C2A127B";

        //string completedStep = "EBACDCBA-0DB9-4582-999B-6317DA0094A7";

        public string GetNextStep(string id, string currentStepId , bool isWaitingList = false, bool isNetworkTeam = false)
        {

            string nextStepId = "";
            switch (currentStepId.ToLower())
            {   
                //welcome
                case "26a7bd18-2e80-4901-9746-8061c7ff569d": nextStepId = nearByNumberStep; break;
                //near by number
                case "7c14e38a-f5cc-46dd-a94e-b49871b819e8": nextStepId = printTemplateStep; break;
                //print template
                case "6895f711-7e76-4043-8777-9267d7cd27ec": nextStepId = findFreeTechnicalStep; break;
                //find free technical
                case "ee7c2188-77a0-406c-baa5-e29925509ae8":

                    if (!isWaitingList && !isNetworkTeam) nextStepId = subscriptionAddressStep;
                    else if (isWaitingList) nextStepId = waitingListStep;
                    else nextStepId = technicalStep;
                    break;
                
                //waiting list
                case "e054ee92-661a-42d2-b0f6-9d78b12331b7": nextStepId = findFreeTechnicalStep; break;
                //network team
                case "73f41594-a8db-4967-acb0-3b8b8e0694a8": nextStepId = reservePhoneNumberStep; break;
                //reserve phone number
                case "64bd8a64-e4e7-4f9c-8497-a466f2b35350": nextStepId = subscriptionAddressStep; break;
                //Address
                case "74fa6d04-804d-45d7-b3e3-887a98450f13": nextStepId = servicesStep; break;
                //services
                case "eec0cf10-f7e2-45ad-953f-a35c1daa4b27":nextStepId = this.IsContactForeigner(id) ? foreignerStep : contractOnHoldStep; break;  //nextStepId = contractOnHoldStep; break;-
                //foreigner step
                case "351b5cf9-ab1e-47e2-bc93-b8b4f101d047": nextStepId = contractOnHoldStep; break;
                //create contract on hold step
                case "3b1e25ea-6751-4bdc-99f8-0cfac2b9a25e": nextStepId = paymentStep; break;
                //payment
                case "6fd4f52a-310d-4e91-999d-f2d6fec19ab0": nextStepId = printContractStep; break;
                //print contract
                case "1b93bb41-aa73-4430-9db0-424243e1ee11": nextStepId = technicalStep; break;
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
