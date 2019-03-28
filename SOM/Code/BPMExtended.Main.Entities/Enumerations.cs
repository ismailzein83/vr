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


    public enum OperationType
    {
        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Subscription")]
        TelephonyLineSubscription = 0,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Termination")]
        TelephonyLineTermination = 1,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Moving Same Switch")]
        TelephonyLineMovingSameSwitch = 2,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Change Phone Number")]
        TelephonyChangePhoneNumber = 3,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Subscription")]
        LeasedLineSubscription = 4,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Subscription")]
        ADSLSubscription = 5,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL for ISP Subscription")]
        ADSLForISPSubscription = 6,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("New Pabx Operation")]
        NewPabxOperation = 7,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Line Moving")]
        ADSLLineMoving = 8,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Line Termination")]
        ADSLLineTermination = 9,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Moving New Switch")]
        TelephonyLineMovingNewSwitch = 10,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Alter Speed")]
        ADSLAlterSpeed = 11,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Change Password")]
        ADSLChangePassword = 12,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("Contract Take Over")]
        ContractTakeOver = 13,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Termination")]
        LeasedLineTermination = 14,

        [LineOfBusiness(LineOfBusiness.GSHDSL)]
        [Description("GSHDSL Subscription")]
        GSHDSLSubscription = 15,

        [LineOfBusiness(LineOfBusiness.GSHDSL)]
        [Description("GSHDSL Termination")]
        GSHDSLTermination = 16,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deported Number")]
        DeportedNumber = 17,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Friend And Family")]
        FriendAndFamily = 18,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Service Addition")]
        ServiceAddition = 19,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Change Speed")]
        ChangeSpeed = 20,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Complaint")]
        ADSLComplaint = 21,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Complaint")]
        TelephonyComplaint = 22,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Complaint")]
        LeasedLineComplaint = 23,

        [LineOfBusiness(LineOfBusiness.Administrative)]
        [Description("Administrative Complaint")]
        AdministrativeComplaint = 24,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Print Configuration")]
        ADSLPrintConfiguration = 25,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Manage PABX")]
        ManagePABX = 26,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deactivate PABX")]
        DeactivatePABX = 27,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Cpt Request")]
        CptRequest = 28,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deactivate Cpt")]
        DeactivateCpt = 29,

        [LineOfBusiness(LineOfBusiness.Administrative)]
        [Description("Create Payment Plan")]
        CreatePaymentPlan = 30,

    }
}
