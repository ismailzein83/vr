-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierMask_Update] 
(
@ID smallint,
@Name nvarchar(200),
@CompanyName nvarchar(200),
@CountryId int,
@RegistrationNumber nvarchar(50),
@VatID nvarchar(50),

@Telephone1 nvarchar(100),
@Telephone2 nvarchar(100),
@Telephone3 nvarchar(100),

@Fax1 nvarchar(100),
@Fax2 nvarchar(100),
@Fax3 nvarchar(100),

@Address1 nvarchar(250),
@Address2 nvarchar(250),
@Address3 nvarchar(250),

@CompanyLogo bigint,
@IsBankReferences bit,

@BillingContact nvarchar(200),
@BillingEmail nvarchar(256),

@PricingContact nvarchar(200),
@PricingEmail nvarchar(256),
@AccountManagerEmail nvarchar(255),

@SupportContact nvarchar(200),
@SupportEmail nvarchar(256),

@CurrencyId varchar(3),

@PriceList nvarchar(200),
@MaskInvoiceformat nvarchar(200),

@MaskOverAllCounter int,
@YearlyMaskOverAllCounter int
)
AS
BEGIN
	UPDATE BEntity.CarrierMask
		SET Name = @Name,
			CompanyName = @CompanyName,
			CountryId = @CountryId,
			RegistrationNumber = @RegistrationNumber,
			VatID = @VatID,
			Telephone1 = @Telephone1,
			Telephone2 = @Telephone2,
			Telephone3 = @Telephone3,
			Fax1 = @Fax1,
			Fax2 = @Fax2,
			Fax3 = @Fax3,
			Address1 = @Address1,
			Address2 = @Address2,
			Address3 = @Address3,
			CompanyLogo = @CompanyLogo,
			IsBankReferences = @IsBankReferences,
			BillingEmail = @BillingEmail,
			BillingContact= @BillingContact,
			
			PricingContact = @PricingContact,
			PricingEmail = @PricingEmail,
			
			AccountManagerEmail = @AccountManagerEmail,
			SupportContact = @SupportContact,
			SupportEmail = @SupportEmail,
			CurrencyId = @CurrencyId,
			PriceList = @PriceList,
			MaskInvoiceformat = @MaskInvoiceformat,
			MaskOverAllCounter = @MaskOverAllCounter,
			YearlyMaskOverAllCounter = @YearlyMaskOverAllCounter
	WHERE ID = @ID

END