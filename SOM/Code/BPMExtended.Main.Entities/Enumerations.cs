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
    public class EntitySchemaNameAttribute : Attribute
    {
        public string schemaName { get; private set; }

        public EntitySchemaNameAttribute(string esn)
        {
            this.schemaName = esn;
        }
    }

    public enum OperationType
    {
        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Subscription")]
        [EntitySchemaName("StLineSubscriptionRequest")]
        TelephonyLineSubscription = 0,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Termination")]
        [EntitySchemaName("StLineTerminationRequest")]
        TelephonyLineTermination = 1,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Moving Same Switch")]
        [EntitySchemaName("StLineMovingSameSwitchRequest")]
        TelephonyLineMovingSameSwitch = 2,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Change Phone Number")]
        [EntitySchemaName("StChangePhoneNumberRequest")]
        TelephonyChangePhoneNumber = 3,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Subscription")]
        [EntitySchemaName("StLeasedLine")]
        LeasedLineSubscription = 4,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Subscription")]
        [EntitySchemaName("StADSL")]
        ADSLSubscription = 5,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL for ISP Subscription")]
        [EntitySchemaName("StISPADSL")]
        ADSLForISPSubscription = 6,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("New Pabx Operation")]
        [EntitySchemaName("StPabx")]
        NewPabxOperation = 7,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Line Moving")]
        [EntitySchemaName("StADSLLineMoving")]
        ADSLLineMoving = 8,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Line Termination")]
        [EntitySchemaName("StADSLLineTermination")]
        ADSLLineTermination = 9,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Moving New Switch")]
        [EntitySchemaName("StLineMovingNewSwitch")]
        TelephonyLineMovingNewSwitch = 10,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Alter Speed")]
        [EntitySchemaName("StADSLAlterSpeed")]
        ADSLAlterSpeed = 11,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Change Password")]
        [EntitySchemaName("StADSLChangePassword")]
        ADSLChangePassword = 12,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("Contract Take Over")]
        [EntitySchemaName("StTelephonyContractTakeOver")]
        ContractTakeOver = 13,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Termination")]
        [EntitySchemaName("StLeasedLineTermination")]
        LeasedLineTermination = 14,

        [LineOfBusiness(LineOfBusiness.GSHDSL)]
        [Description("GSHDSL Subscription")]
        [EntitySchemaName("StGSHDSL")]
        GSHDSLSubscription = 15,

        [LineOfBusiness(LineOfBusiness.GSHDSL)]
        [Description("GSHDSL Termination")]
        [EntitySchemaName("StGSHDSLTermination")]
        GSHDSLTermination = 16,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deported Number")]
        [EntitySchemaName("StDeportedNumber")]
        DeportedNumber = 17,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Friend And Family")]
        [EntitySchemaName("StFriendAndFamily")]
        FriendAndFamily = 18,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Service Addition")]
        [EntitySchemaName("StServiceAddition")]
        ServiceAddition = 19,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Change Speed")]
        [EntitySchemaName("StChangeLeasedLineSpeed")]
        ChangeSpeed = 20,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Complaint")]
        [EntitySchemaName("StADSLComplaint")]
        ADSLComplaint = 21,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Complaint")]
        [EntitySchemaName("")]
        TelephonyComplaint = 22,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Complaint")]
        [EntitySchemaName("")]
        LeasedLineComplaint = 23,

        [LineOfBusiness(LineOfBusiness.Administrative)]
        [Description("Administrative Complaint")]
        [EntitySchemaName("")]
        AdministrativeComplaint = 24,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Print Configuration")]
        [EntitySchemaName("StADSLPrintConfiguration")]
        ADSLPrintConfiguration = 25,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Manage PABX")]
        [EntitySchemaName("StManagePabx")]
        ManagePABX = 26,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deactivate PABX")]
        [EntitySchemaName("StDeactivatePabx")]
        DeactivatePABX = 27,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Cpt Request")]
        [EntitySchemaName("StCpt")]
        CptRequest = 28,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deactivate Cpt")]
        [EntitySchemaName("StDeactivateCpt")]
        DeactivateCpt = 29,

        [LineOfBusiness(LineOfBusiness.Administrative)]
        [Description("Create Payment Plan")]
        [EntitySchemaName("StCreatePaymentPlan")]
        CreatePaymentPlan = 30,

    }
}
