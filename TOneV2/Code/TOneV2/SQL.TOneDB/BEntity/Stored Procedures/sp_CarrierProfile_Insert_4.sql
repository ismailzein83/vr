-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierProfile_Insert] 
(

@Name nvarchar(200),
@CompanyName nvarchar(200),
@Country nvarchar(100),
@City nvarchar(100),
@RegistrationNumber nvarchar(50),
@Telephone nvarchar(100),
@Fax nvarchar(100),
@Address1 nvarchar(250),
@Address2 nvarchar(250),
@Address3 nvarchar(250),
@Website varchar(255),
@BillingEmail nvarchar(256),
@BillingContact nvarchar(200),
@BillingDisputeEmail nvarchar(256),
@PricingContact nvarchar(200),
@PricingEmail nvarchar(256),
@AccountManagerEmail nvarchar(255),
@AccountManagerContact nvarchar(200),
@SupportContact nvarchar(200),
@SupportEmail nvarchar(256),
@TechnicalContact nvarchar(200),
@TechnicalEmail nvarchar(256),
@CommercialContact nvarchar(200),
@CommercialEmail nvarchar(250),
@SMSPhoneNumber varchar(50),
@ProfileId smallint OUTPUT
)
AS
BEGIN
	INSERT INTO CarrierProfile
		 (	Name ,
			CompanyName,
			Country,
			City,
			RegistrationNumber,
			Telephone,
			Fax,
			Address1,
			Address2,
			Address3,
			Website,
			BillingEmail,
			BillingContact,
			BillingDisputeEmail,
			PricingContact,
			PricingEmail,
			AccountManagerEmail,
			AccountManagerContact,
			SupportContact,
			SupportEmail,
			TechnicalContact,
			TechnicalEmail,
			CommercialContact,
			CommercialEmail,
			SMSPhoneNumber)
			VALUES
			( @Name,
			  @CompanyName,
			  @Country,
			  @City,
			  @RegistrationNumber,
			  @Telephone,
			  @Fax,
			  @Address1,
			  @Address2,
			  @Address3,
			  @Website,
			  @BillingEmail,
			  @BillingContact,
			  @BillingDisputeEmail,
			  @PricingContact,
			  @PricingEmail,
			  @AccountManagerEmail,
			  @AccountManagerContact,
			  @SupportContact,
			  @SupportEmail,
			  @TechnicalContact,
			  @TechnicalEmail,
			  @CommercialContact,
			  @CommercialEmail,
			  @SMSPhoneNumber
			)
set @ProfileId = scope_Identity()
END