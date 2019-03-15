using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public enum BPMCustomerType { Residential = 0, Enterprise = 1, Offical = 2 }
    public enum LineOfBusiness
    {
        Telephony = 0,

        LeasedLine = 1,

        ADSL = 2,

        GSHDSL = 3,

        Administrative = 4

    }
    public class LineOfBusinessAttribute : Attribute
    {
        public LineOfBusiness LOB { get; private set; }

        public LineOfBusinessAttribute(LineOfBusiness lob)
        {
            this.LOB = lob;
        }
    }

    public enum OperationType
    {
        [LineOfBusiness(LineOfBusiness.Telephony)]
        TelephonyLineSubscription = 0,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        TelephonyLineTermination = 1,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        TelephonyLineMovingSameSwitch = 2,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        TelephonyChangePhoneNumber = 3,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        LeasedLineSubscription = 4,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ADSLSubscription = 5,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ADSLForISPSubscription = 6,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        NewPabxOperation = 7,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ADSLLineMoving = 8,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ADSLLineTermination = 9,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        TelephonyLineMovingNewSwitch = 10,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ADSLAlterSpeed = 11,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ADSLChangePassword = 12,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ContractTakeOver = 13,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        LeasedLineTermination = 14,

        [LineOfBusiness(LineOfBusiness.GSHDSL)]
        GSHDSLSubscription = 15,

        [LineOfBusiness(LineOfBusiness.GSHDSL)]
        GSHDSLTermination = 16,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        DeportedNumber = 17,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        FriendAndFamily = 18,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        ServiceAddition = 19,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        ChangeSpeed = 20,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ADSLComplaint = 21,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        TelephonyComplaint = 22,

        [LineOfBusiness(LineOfBusiness.LeasedLine)]
        LeasedLineComplaint = 23,

        [LineOfBusiness(LineOfBusiness.Administrative)]
        AdministrativeComplaint = 24,

        [LineOfBusiness(LineOfBusiness.ADSL)]
        ADSLPrintConfiguration = 25,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        ManagePABX = 26,

        [LineOfBusiness(LineOfBusiness.Telephony)]
        DeactivatePABX = 27,

    }
}
