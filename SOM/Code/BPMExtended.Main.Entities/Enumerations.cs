using SOM.Main.Entities;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public enum BPMCustomerType { Residential = 0, Enterprise = 1, Offical = 2 }
    public enum DirectoryInquiry { NoAction = 0, Add = 1, Remove = 2 }

    public enum ContacrtOnHoldStatus { New = 0, Postponed = 5, Running = 10, Waiting = 20, Cancelling = 30, Completed = 50, Aborted = 60, Suspended = 70, Terminated = 80, Cancelled=90 }

    public class EntitySchemaNameAttribute : Attribute
    {
        public string schemaName { get; private set; }

        public EntitySchemaNameAttribute(string esn)
        {
            this.schemaName = esn;
        }
    }
    public class CompletedStepAttribute : Attribute
    {
        public string CompletedStep { get; private set; }

        public CompletedStepAttribute (string completed)
        {
            this.CompletedStep = completed;
        }
    }
    public class CompletedStepIdAttribute : Attribute
    {
        public string CompletedStepId { get; private set; }

        public CompletedStepIdAttribute(string completedId)
        {
            this.CompletedStepId = completedId;
        }
    }
    public enum OperationType
    {
        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Subscription")]
        [EntitySchemaName("StLineSubscriptionRequest")]
        [CompletedStep("")]
        [CompletedStepId("")]
        TelephonyLineSubscription = 0,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Termination")]
        [EntitySchemaName("StLineTerminationRequest")]
        [CompletedStep("")]
        [CompletedStepId("")]
        TelephonyLineTermination = 1,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Moving Same Switch")]
        [EntitySchemaName("StLineMovingSameSwitchRequest")]
        [CompletedStep("")]
        [CompletedStepId("")]
        TelephonyLineMovingSameSwitch = 2,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Change Phone Number")]
        [EntitySchemaName("StChangePhoneNumberRequest")]
        [CompletedStep("")]
        [CompletedStepId("")]
        TelephonyChangePhoneNumber = 3,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Subscription")]
        [EntitySchemaName("StLeasedLine")]
        [CompletedStep("")]
        [CompletedStepId("")]
        LeasedLineSubscription = 4,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Subscription")]
        [EntitySchemaName("StADSL")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLSubscription = 5,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL for ISP Subscription")]
        [EntitySchemaName("StISPADSL")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLForISPSubscription = 6,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("New Pabx Operation")]
        [EntitySchemaName("StPabx")]
        [CompletedStep("StStepId")]
        [CompletedStepId("1386C350-017D-400B-855C-04FC427868BF")]
        NewPabxOperation = 7,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Line Moving")]
        [EntitySchemaName("StADSLLineMoving")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLLineMoving = 8,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Line Termination")]
        [EntitySchemaName("StADSLLineTermination")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLLineTermination = 9,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Moving New Switch")]
        [EntitySchemaName("StLineMovingNewSwitch")]
        [CompletedStep("")]
        [CompletedStepId("")]
        TelephonyLineMovingNewSwitch = 10,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Alter Speed")]
        [EntitySchemaName("StADSLAlterSpeed")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLAlterSpeed = 11,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Change Password")]
        [EntitySchemaName("StADSLChangePassword")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLChangePassword = 12,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("Contract Take Over")]
        [EntitySchemaName("StTelephonyContractTakeOver")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ContractTakeOver = 13,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Termination")]
        [EntitySchemaName("StLeasedLineTermination")]
        [CompletedStep("")]
        [CompletedStepId("")]
        LeasedLineTermination = 14,

        [LineOfBusiness(LineOfBusiness.GSHDSL)]
        [Description("GSHDSL Subscription")]
        [EntitySchemaName("StGSHDSL")]
        [CompletedStep("")]
        [CompletedStepId("")]
        GSHDSLSubscription = 15,

        [LineOfBusiness(LineOfBusiness.GSHDSL)]
        [Description("GSHDSL Termination")]
        [EntitySchemaName("StGSHDSLTermination")]
        [CompletedStep("")]
        [CompletedStepId("")]
        GSHDSLTermination = 16,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deported Number")]
        [EntitySchemaName("StDeportedNumber")]
        [CompletedStep("")]
        [CompletedStepId("")]
        DeportedNumber = 17,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Friend And Family")]
        [EntitySchemaName("StFriendAndFamily")]
        [CompletedStep("")]
        [CompletedStepId("")]
        FriendAndFamily = 18,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Service Addition")]
        [EntitySchemaName("StServiceAddition")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ServiceAddition = 19,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Change Speed")]
        [EntitySchemaName("StChangeLeasedLineSpeed")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ChangeSpeed = 20,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Complaint")]
        [EntitySchemaName("StADSLComplaint")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLComplaint = 21,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Complaint")]
        [EntitySchemaName("")]
        [CompletedStep("")]
        [CompletedStepId("")]
        TelephonyComplaint = 22,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Complaint")]
        [EntitySchemaName("")]
        [CompletedStep("")]
        [CompletedStepId("")]
        LeasedLineComplaint = 23,

        [LineOfBusiness(LineOfBusiness.Administrative)]
        [Description("Administrative Complaint")]
        [EntitySchemaName("")]
        [CompletedStep("")]
        [CompletedStepId("")]
        AdministrativeComplaint = 24,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Print Configuration")]
        [EntitySchemaName("StADSLPrintConfiguration")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLPrintConfiguration = 25,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Manage PABX")]
        [EntitySchemaName("StManagePabx")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ManagePABX = 26,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deactivate PABX")]
        [EntitySchemaName("StDeactivatePabx")]
        [CompletedStep("")]
        [CompletedStepId("")]
        DeactivatePABX = 27,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Cpt Request")]
        [EntitySchemaName("StCpt")]
        [CompletedStep("")]
        [CompletedStepId("")]
        CptRequest = 28,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deactivate Cpt")]
        [EntitySchemaName("StDeactivateCpt")]
        [CompletedStep("")]
        [CompletedStepId("")]
        DeactivateCpt = 29,

        [LineOfBusiness(LineOfBusiness.Administrative)]
        [Description("Create Payment Plan")]
        [EntitySchemaName("StCreatePaymentPlan")]
        [CompletedStep("")]
        [CompletedStepId("")]
        CreatePaymentPlan = 30,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Change Rate Plan")]
        [EntitySchemaName("StChangeRatePlan")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ChangeRatePlan = 31,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Update Contract Address")]
        [EntitySchemaName("StUpdateContractAddress")]
        [CompletedStep("StStepId")]
        [CompletedStepId("4AFB7452-F5FF-4CB7-8AE3-79801A6DEB7F")]
        UpdateContractAddress = 32,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Network Reset Password")]
        [EntitySchemaName("StNetworkResetPassword")]
        [CompletedStep("")]
        [CompletedStepId("")]
        NetworkResetPassword = 33,


        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Other Charges")]
        [EntitySchemaName("StOtherCharges")]
        [CompletedStep("")]
        [CompletedStepId("")]
        OtherCharges = 34,


        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Line Blocking")]
        [EntitySchemaName("StLineBlocking")]
        [CompletedStep("")]
        [CompletedStepId("")]
        LineBlocking = 35,


        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Line Unblocking")]
        [EntitySchemaName("StLineUnblocking")]
        [CompletedStep("")]
        [CompletedStepId("")]
        LineUnblocking = 36,

    }
    public enum ContractStatus
    {
       
        [LookupIdAttribute("EE96CD5C-0792-47BE-A4C1-04A24430BED7")]
        OnHold = 1,
        [LookupIdAttribute("86262707-1F22-4DD4-AD06-48714DBEC34C")]
        Active = 2,
        [LookupIdAttribute("C2F6C676-29E2-44F0-A12F-9B4C62C94A40")]
        Suspended = 3,
        [LookupIdAttribute("00F5E2B9-B84E-497A-A0C7-E15CBD75009B")]
        Inactive = 4,

    }
    public class LookupIdAttribute : Attribute
    {
        public string LookupId { get; private set; }

        public LookupIdAttribute(string lookupId)
        {
            this.LookupId = lookupId;
        }
    }
}
