﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class TelephonyContractTakeOverFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "496681F9-8A5D-4CAD-89E3-2918B8A3EB5D";
        const string targetCustomerStep = "6491A4EF-29F6-4879-BAD8-30884F621065";
        const string printStep = "056AC63F-B743-4A7E-B007-371014BEF79F";
        const string billOnDemandStep = "6430317A-87B7-4703-AEAF-38E8B00A6479";
        const string paymentStep = "90ED9157-3D6E-41F8-BCA4-41845BF6BBDD";
        const string adslCredentialStep = "0ED21643-2A5C-4CB2-8320-64B67D30D516";
        const string attachmentStep = "14D4D15B-4F29-4E44-8F9C-BCB1059DE003";
        const string submitToOMStep = "022B65D1-CF77-440B-A097-F87212AFF146";
        const string technicalStep = "DF7AC0B3-F53D-4104-A260-2509A3E08769";
        const string completedStep = "A826F4E8-9352-46FB-814E-8B0B52655659";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = targetCustomerStep; break;
                case targetCustomerStep: nextStepId = printStep; break;
                //case printStep: nextStepId = checkADSLContract(id)? adslCredentialStep : paymentStep; break;
                //case adslCredentialStep: nextStepId = paymentStep; break;
                //case paymentStep: nextStepId = technicalStep; break;
                    
                case printStep: nextStepId = new ContractTakeOverManager().checkBillOnDemand(id) ?billOnDemandStep:new ContractTakeOverManager().hasADSLContract(id)? adslCredentialStep : paymentStep; break;
                case billOnDemandStep: nextStepId = new ContractTakeOverManager().hasADSLContract(id)? adslCredentialStep : paymentStep; break;
                case adslCredentialStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = submitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }

       


    }
}
