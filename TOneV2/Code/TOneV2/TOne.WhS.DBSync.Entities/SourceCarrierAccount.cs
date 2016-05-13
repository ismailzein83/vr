using System;
namespace TOne.WhS.DBSync.Entities
{
    public class SourceCarrierAccount : Vanrise.Entities.EntitySynchronization.ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string NameSuffix { get; set; }

        public Int16 ProfileId { get; set; }


        //CarrierAccountID	varchar(5)	Unchecked
        //ProfileID	smallint	Unchecked
        //ServicesFlag	smallint	Unchecked
        //ActivationStatus	tinyint	Unchecked
        //RoutingStatus	tinyint	Unchecked
        //AccountType	tinyint	Unchecked
        //CustomerPaymentType	tinyint	Unchecked
        //SupplierPaymentType	tinyint	Checked
        //SupplierCreditLimit	int	Checked
        //BillingCycleFrom	smallint	Checked
        //BillingCycleTo	smallint	Checked
        //GMTTime	smallint	Checked
        //IsTOD	char(1)	Checked
        //IsDeleted	char(1)	Checked
        //IsOriginatingZonesEnabled	char(1)	Checked
        //Notes	text	Checked
        //NominalCapacityInE1s	int	Checked
        //UserID	int	Checked
        //timestamp	timestamp	Checked
        //CarrierGroupID	int	Checked
        //RateIncreaseDays	int	Checked
        //BankReferences	varchar(MAX)	Checked
        //CustomerCreditLimit	int	Checked
        //IsPassThroughCustomer	char(1)	Checked
        //IsPassThroughSupplier	char(1)	Checked
        //RepresentsASwitch	char(1)	Checked
        //IsAToZ	char(1)	Checked
        //NameSuffix	nvarchar(100)	Checked
        //SupplierRatePolicy	int	Checked
        //CustomerGMTTime	smallint	Checked
        //ImportEmail	varchar(320)	Checked
        //ImportSubjectCode	varchar(255)	Checked
        //IsNettingEnabled	char(1)	Checked
        //Services	numeric(6, 2)	Checked
        //ConnectionFees	money	Checked
        //CustomerActivateDate	datetime	Checked
        //CustomerDeactivateDate	datetime	Checked
        //SupplierActivateDate	datetime	Checked
        //SupplierDeactivateDate	datetime	Checked
        //InvoiceSerialPattern	nvarchar(300)	Checked
        //CustomerMask	varchar(10)	Checked
        //IsCustomerCeiling	char(1)	Checked
        //IsSupplierCeiling	char(1)	Checked
        //CarrierGroups	varchar(50)	Checked
        //CodeView	int	Checked
        //IsCustomCodeView	char(1)	Checked
        //IsProduct	char(1)	Checked
        //AutomaticInvoiceSettingID	int	Checked
        //CustomerSMSOnPayment	char(1)	Checked
        //CustomerMailOnPayment	char(1)	Checked
        //SupplierSMSOnPayment	char(1)	Checked
        //SupplierMailOnPayment	char(1)	Checked
        //IsCustomDispute	char(1)	Checked
        //CustomerAllowPayment	char(1)	Checked
        //SupplierAllowPayment	char(1)	Checked
        //AutomatedImportSubjectCode	varchar(255)	Checked
        //CarrierMask	varchar(50)	Checked
        //IsLCREnabled	char(1)	Checked
        //DS_ID_auto	int	Unchecked



    }
}
