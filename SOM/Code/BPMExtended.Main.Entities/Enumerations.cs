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
    public enum ResultStatus { Error = 0, Success = 1, Warning = 2 }
    public enum ADSLSubType { None = 0, Internet= 1, VPN = 2 }
    public enum ContacrtOnHoldStatus { New = 0, Postponed = 5, Running = 10, Waiting = 20, Cancelling = 30, Completed = 50, Aborted = 60, Suspended = 70, Terminated = 80, Cancelled=90 }
    public enum ValidatePaymentState
    {
        Valid = 1,
        CustomerBalanceError = 2,
        CashFeesError = 3,
        DepositError = 4,
        Undefined = 5
    }

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
    public class TechnicalStepIdAttribute : Attribute
    {
        public string technicalStepId { get; private set; }

        public TechnicalStepIdAttribute(string technicalStepId)
        {
            this.technicalStepId = technicalStepId;
        }
    }
    public class TechnicalStepFieldNameAttribute : Attribute
    {
        public string fieldName { get; private set; }

        public TechnicalStepFieldNameAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }
    }
    public enum OperationType
    {
        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Subscription")]
        [EntitySchemaName("StLineSubscriptionRequest")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("5AC289E9-1489-423E-B6E0-32479C2A127B")]
        [CompletedStep("StStepId")]
        [CompletedStepId("EBACDCBA-0DB9-4582-999B-6317DA0094A7")]
        TelephonyLineSubscription = 0,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Termination")]
        [EntitySchemaName("StLineTerminationRequest")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("45363147-CC59-4632-B09E-EB850D2FD25F")]
        [CompletedStep("StStepId")]
        [CompletedStepId("CEBCB883-84AA-4183-9938-817E711EB2BF")]
        TelephonyLineTermination = 1,
         
        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Moving Same Switch")]
        [EntitySchemaName("StLineMovingSameSwitchRequest")]
        [CompletedStep("StStepId")]
        [CompletedStepId("15F6B32E-978A-421E-9FDF-CCA45D422AB8")]
        TelephonyLineMovingSameSwitch = 2,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Change Phone Number")]
        [EntitySchemaName("StChangePhoneNumberRequest")]
        [CompletedStep("StStepId")]
        [CompletedStepId("A359D1DA-9970-4C97-A669-8F30E7B31550")]
        TelephonyChangePhoneNumber = 3,

        //[LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Subscription")]
        [EntitySchemaName("StLeasedLine")]
        [CompletedStep("StStepId")]
        [CompletedStepId("DBEF5609-03E6-47D9-A901-6F48914A57F3")]
        LeasedLineSubscription = 4,

        //[LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Subscription")]
        [EntitySchemaName("StADSL")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("6AB2B0E3-5201-4189-9838-CF4B4E4E17D9  ")]
        [CompletedStep("StStepId")]
        [CompletedStepId("8CD5BDC5-2551-4767-8166-5D334D5E0FD7")]
        ADSLSubscription = 5,

        //[LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL for ISP Subscription")]
        [EntitySchemaName("StISPADSL")]
        [CompletedStep("StStepId")]
        [CompletedStepId("72EE0965-4F1F-4A3E-A78F-02DBDD96F168")]
        ADSLForISPSubscription = 6,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("New Pabx Operation")]
        [EntitySchemaName("StPabx")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("4AD46645-AD4A-4F53-83DC-EB38831867FA")]
        [CompletedStep("StStepId")]
        [CompletedStepId("1386C350-017D-400B-855C-04FC427868BF")]
        NewPabxOperation = 7,

        //[LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Line Moving")]
        [EntitySchemaName("StADSLLineMoving")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("9A9C358C-8A33-4580-A5BE-36126E805E3E")]
        [CompletedStep("StStepId")]
        [CompletedStepId("F3384A9D-A1D3-4F2C-8985-8987639522DE")]
        ADSLLineMoving = 8,

        //[LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Line Termination")]
        [EntitySchemaName("StADSLLineTermination")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("56D55B17-8962-4D80-9564-361AD127E6B5")]
        [CompletedStep("StStepId")]
        [CompletedStepId("639E4A4C-1752-42E3-83CD-85674B6E9903")]
        ADSLLineTermination = 9,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Line Moving New Switch")]
        [EntitySchemaName("StLineMovingNewSwitch")]
        [CompletedStep("")]
        [CompletedStepId("")]
        TelephonyLineMovingNewSwitch = 10,

        //[LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Alter Speed")]
        [EntitySchemaName("StADSLAlterSpeed")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("B0DBAD29-F63F-41E3-98FE-8EA06B57B9A7")]
        [CompletedStep("StStepId")]
        [CompletedStepId("33AF8121-E09C-4F57-AFA9-1FD79939580E")]
        ADSLAlterSpeed = 11,

        //[LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Change Password")]
        [EntitySchemaName("StADSLChangePassword")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("3F70A8B3-DAD2-4428-80C2-EABB8C19CD55")]
        [CompletedStep("StStepId")]
        [CompletedStepId("E326FAEA-C8F9-497E-85E0-3B92CFC32231")]
        ADSLChangePassword = 12,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Contract Take Over")]
        [EntitySchemaName("StTelephonyContractTakeOver")]
        [CompletedStep("StStepId")]
        [CompletedStepId("A826F4E8-9352-46FB-814E-8B0B52655659")]
        ContractTakeOver = 13,

        //[LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Termination")]
        [EntitySchemaName("StLeasedLineTermination")]
        [CompletedStep("StStepId")]
        [CompletedStepId("94CA2192-D445-47D8-80C5-BF26E462CFC6")]
        LeasedLineTermination = 14,

        //[LineOfBusiness(LineOfBusiness.GSHDSL)]
        [Description("GSHDSL Subscription")]
        [EntitySchemaName("StGSHDSL")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("4A64F6DD-F60A-4D7D-ABB8-CC1AE8D501F6")]
        [CompletedStep("StStepId")]
        [CompletedStepId("2B0CDA46-E3DF-449B-A4A1-C677781CF4B9")]
        GSHDSLSubscription = 15,

        //[LineOfBusiness(LineOfBusiness.GSHDSL)]
        [Description("GSHDSL Termination")]
        [EntitySchemaName("StGSHDSLTermination")]
        [CompletedStep("")]
        [CompletedStepId("")]
        GSHDSLTermination = 16,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deported Number")]
        [EntitySchemaName("StDeportedNumber")]
        [CompletedStep("StStepId")]
        [CompletedStepId("5442DDB1-82C0-4763-9BE8-DB8C6AD58B7A")]
        DeportedNumber = 17,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Friend And Family")]
        [EntitySchemaName("StFriendAndFamily")]
        [CompletedStep("StStepId")]
        [CompletedStepId("498A1C06-03A9-4EB7-A3BF-B0CC6554C40A")]
        FriendAndFamily = 18,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Service Addition")]
        [EntitySchemaName("StServiceAdditionRequest")]
        [CompletedStep("StStepId")]
        [CompletedStepId("64436D8D-547F-4B12-AFAA-B44425FE6EAF")]
        ServiceAddition = 19,

        //[LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Change Speed")]
        [EntitySchemaName("StChangeLeasedLineSpeed")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ChangeSpeed = 20,

        //[LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Complaint")]
        [EntitySchemaName("StADSLComplaint")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLComplaint = 21,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Telephony Complaint")]
        [EntitySchemaName("")]
        [CompletedStep("")]
        [CompletedStepId("")]
        TelephonyComplaint = 22,

        //[LineOfBusiness(LineOfBusiness.LeasedLine)]
        [Description("Leased Line Complaint")]
        [EntitySchemaName("")]
        [CompletedStep("")]
        [CompletedStepId("")]
        LeasedLineComplaint = 23,

        //[LineOfBusiness(LineOfBusiness.Administrative)]
        [Description("Administrative Complaint")]
        [EntitySchemaName("")]
        [CompletedStep("")]
        [CompletedStepId("")]
        AdministrativeComplaint = 24,

        //[LineOfBusiness(LineOfBusiness.ADSL)]
        [Description("ADSL Print Configuration")]
        [EntitySchemaName("StADSLPrintConfiguration")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ADSLPrintConfiguration = 25,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Manage PABX")]
        [EntitySchemaName("StManagePabx")]
        [CompletedStep("")]
        [CompletedStepId("")]
        ManagePABX = 26,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deactivate PABX")]
        [EntitySchemaName("StDeactivatePabx")]
        [CompletedStep("")]
        [CompletedStepId("")]
        DeactivatePABX = 27,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Cpt Request")]
        [EntitySchemaName("StCpt")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("A8F7C0E8-4903-4343-93E4-87295BF4D64E")]
        [CompletedStep("StStepId")]
        [CompletedStepId("2A290E4F-A034-46A5-95BF-E0C8ED760805")]
        CptRequest = 28,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Deactivate Cpt")]
        [EntitySchemaName("StDeactivateCpt")]
        [CompletedStep("")]
        [CompletedStepId("")]
        DeactivateCpt = 29,

        //[LineOfBusiness(LineOfBusiness.Administrative)]
        [Description("Create Payment Plan")]
        [EntitySchemaName("StCreatePaymentPlan")]
        [CompletedStep("")]
        [CompletedStepId("")]
        CreatePaymentPlan = 30,

        ///[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Change Rate Plan")]
        [EntitySchemaName("StChangeRatePlan")]
        [CompletedStep("StStepId")]
        [CompletedStepId("1F05083D-6074-47DD-822D-ED30362E30BE")]
        ChangeRatePlan = 31,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Update Contract Address")]
        [EntitySchemaName("StUpdateContractAddress")]
        [CompletedStep("StStepId")]
        [CompletedStepId("4AFB7452-F5FF-4CB7-8AE3-79801A6DEB7F")]
        UpdateContractAddress = 32,

        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Network Reset Password")]
        [EntitySchemaName("StNetwrokResetKeyword")]
        [CompletedStep("StStepId")]
        [CompletedStepId("9730D12A-23FB-45D9-AB4E-126B1028BE91")]
        [TechnicalStepFieldName("StResetKeywordTechnicalStepId")]
        [TechnicalStepId("D3D55306-886B-4087-8DEC-8249D28EF7CB")]
        NetworkResetPassword = 33,


        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Other Charges")]
        [EntitySchemaName("StOtherCharges")]
        [CompletedStep("")]
        [CompletedStepId("")]
        OtherCharges = 34,


        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Line Blocking")]  
        [EntitySchemaName("StLineBlock")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("9ACA0E2B-4D75-4F13-A486-C2674D2F8B0F")]
        LineBlocking = 35,


        //[LineOfBusiness(LineOfBusiness.Telephony)]
        [Description("Line Unblocking")]
        [EntitySchemaName("StLineUnblock")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("A7845FA8-8454-492E-908C-90B9E309C9FB")]
        LineUnblocking = 36,
        
        [Description("Update Customer Category")]
        [EntitySchemaName("StUpdateCustomerCategory")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("9434699C-CA76-498F-A245-B993E3A3756F")]
        UpdateCustomerCategory = 37,
        
        [Description("Update Customer Address")]
        [EntitySchemaName("StUpdateCustomerAddress")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("3CFA38E1-C382-440F-B21B-6B27274CF38B")]
        UpdateCustomerAddress = 38,

        [Description("Update Customer Profile")]
        [EntitySchemaName("StUpdateCustomerProfile")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("0E818E54-C363-4EE2-8D9D-FD41D9D5A5B1")]
        UpdateCustomerProfile = 39,

        [Description("Line Moving Request")]
        [EntitySchemaName("StLineMovingRequest")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("B2B4E3AF-B99D-42E9-9E6E-717D145F3258")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("BB0D3D76-4029-4517-B6BD-A23DA24A9F7C")]
        LineMovingRequest = 40,

        [Description("Service Removal")]
        [EntitySchemaName("StServiceRemoval")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("42CC62BA-A708-4097-A2B2-17CC2374AC13")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("B1295B96-C7B3-40B3-9089-24A05E94FF41")]
        ServiceRemoval = 41,

        [Description("Call Details")]
        [EntitySchemaName("StCallDetailsRequest")]
        [CompletedStep("StStepId")]
        [CompletedStepId("0E6F81C9-5F92-4FDF-ABAB-5C39B18E49E9")]
        CallDetails = 42,

        [Description("Update Payment Arrangement")]
        [EntitySchemaName("StUpdatePaymentArrangement")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("0BAD833F-CFC0-4C9D-8446-C2F956CCAC3A")]
        UpdatePaymentArrangement = 43,

        [Description("Last Mile Change")]
        [EntitySchemaName("StLastMileChange")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("BDFC6820-9932-4FE1-A628-D3537EFF100E")]
        LastMileChange = 44,

        [Description("ADSL Take Over")]
        [EntitySchemaName("StADSLContractTakeOver")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("FB364A70-F0C4-4B0F-AE4A-6A38A9189C79")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("1E0E1552-A872-4C70-8EC5-49E606FA09CD")]
        ADSLTakeOver = 45,

        [Description("Suspension")]
        [EntitySchemaName("StSuspension")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("8340ad42-0c1e-4063-ba73-74b3cd917c2f")]
        Suspension = 46,

        [Description("Revoke")]
        [EntitySchemaName("StRevoke")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("5e803dda-f776-4ddd-a971-345d87a99d96")]
        Revoke = 47,

        [Description("Telephony No Cabling")]
        [EntitySchemaName("StTelephonyNoCabling")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("3257c859-885b-410e-a3a2-fe99e0723a62")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("ee43e28d-aa12-41a3-8c97-626dcaad95b0")]
        TelephonyNoCabling = 48,

        [Description("BSCSSuspensionFromDunning")]
        [EntitySchemaName("StBSCSSuspensionFromDunning")]
        [TechnicalStepFieldName("StWorkOrderStageId")]
        [TechnicalStepId("04AFF621-689F-47CD-8084-837B976D743E")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("2F9ECCE9-DFD7-4F15-9FED-61483A30534A")]
        BSCSSuspensionFromDunning =49,

        [Description("Bill On Demand Operation")]
        [EntitySchemaName("StBillOnDemandOperation")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("ee43e28d-aa12-41a3-8c97-626dcaad95b0")]
        BillOnDemandOperation = 50,

        [Description("Update Account")]
        [EntitySchemaName("StUpdateAccount")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("9129190F-6B33-43DC-B404-3198273D3962")]
        UpdateAccount = 51,

        [Description("Update Account Official")]
        [EntitySchemaName("StUpdateAccountOfficial")]
        [CompletedStep("StTypeId")]
        [CompletedStepId("352ABB46-323D-4734-A479-67D2F403BDB9")]
        UpdateOfficialAccount = 52,
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
