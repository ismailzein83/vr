
namespace TOne.WhS.DBSync.Entities
{
    public class SourceCarrierProfile : Vanrise.Entities.EntitySynchronization.ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }


        //        ProfileID	smallint	Unchecked
        //Name	nvarchar(200)	Checked
        //CompanyName	nvarchar(200)	Checked
        //CompanyLogo	image	Checked
        //CompanyLogoName	nvarchar(500)	Checked
        //Address1	nvarchar(250)	Checked
        //Address2	nvarchar(100)	Checked
        //Address3	nvarchar(100)	Checked
        //Country	nvarchar(100)	Checked
        //City	nvarchar(100)	Checked
        //Telephone	nvarchar(100)	Checked
        //Fax	nvarchar(100)	Checked
        //BillingContact	nvarchar(200)	Checked
        //BillingEmail	nvarchar(256)	Checked
        //PricingContact	nvarchar(200)	Checked
        //PricingEmail	nvarchar(256)	Checked
        //SupportContact	nvarchar(200)	Checked
        //SupportEmail	nvarchar(256)	Checked
        //CurrencyID	varchar(3)	Checked
        //DuePeriod	tinyint	Checked
        //PaymentTerms	tinyint	Checked
        //Tax1	numeric(6, 2)	Checked
        //Tax2	numeric(6, 2)	Checked
        //IsTaxAffectsCost	char(1)	Checked
        //TaxFormula	varchar(200)	Checked
        //VAT	numeric(6, 2)	Checked
        //Services	numeric(6, 2)	Checked
        //ConnectionFees	money	Checked
        //IsDeleted	char(1)	Checked
        //BankingDetails	ntext	Checked
        //UserID	int	Checked
        //timestamp	timestamp	Checked
        //RegistrationNumber	nvarchar(50)	Checked
        //EscalationLevel	nvarchar(MAX)	Checked
        //Guarantee	float	Checked
        //CustomerPaymentType	tinyint	Checked
        //SupplierPaymentType	tinyint	Checked
        //CustomerCreditLimit	int	Checked
        //SupplierCreditLimit	int	Checked
        //IsNettingEnabled	char(1)	Checked
        //CustomerActivateDate	datetime	Checked
        //CustomerDeactivateDate	datetime	Checked
        //SupplierActivateDate	datetime	Checked
        //SupplierDeactivateDate	datetime	Checked
        //VatID	varchar(20)	Checked
        //VatOffice	varchar(50)	Checked
        //AccountManagerEmail	nvarchar(255)	Checked
        //InvoiceByProfile	char(1)	Checked
        //SMSPhoneNumber	varchar(50)	Checked
        //CustomerSMSOnPayment	char(1)	Checked
        //CustomerMailOnPayment	char(1)	Checked
        //SupplierSMSOnPayment	char(1)	Checked
        //SupplierMailOnPayment	char(1)	Checked
        //Website	varchar(255)	Checked
        //BillingDisputeEmail	nvarchar(256)	Checked
        //CustomerAllowPayment	char(1)	Checked
        //SupplierAllowPayment	char(1)	Checked
        //TechnicalContact	nvarchar(200)	Checked
        //TechnicalEmail	nvarchar(256)	Checked
        //CommercialContact	nvarchar(200)	Checked
        //CommercialEmail	nvarchar(250)	Checked
        //AccountManagerContact	nvarchar(200)	Checked



    }
}
