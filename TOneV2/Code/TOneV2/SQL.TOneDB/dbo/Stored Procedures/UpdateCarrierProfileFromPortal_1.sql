
CREATE Proc [dbo].[UpdateCarrierProfileFromPortal]
(
		
       @ProfileID int
      ,@Name nvarchar(200)=null
      ,@CompanyName nvarchar(50)=NULL
      ,@CompanyLogoName nvarchar(50)=null
      ,@Address1 nvarchar(250)=null
      ,@Address2 nvarchar(100)=null
      ,@Address3 nvarchar(100)=null
      ,@City nvarchar(100)=null
      ,@Country nvarchar(100)=null
      ,@Telephone nvarchar(100)=null
      ,@Fax nvarchar(100)=null
      ,@BillingContact nvarchar(200)
      ,@BillingEmail nvarchar(256)
      ,@PricingContact nvarchar(200)
      ,@PricingEmail nvarchar(256)
      ,@SupportContact nvarchar(200)
      ,@SupportEmail nvarchar(256)
      ,@CurrencyID varchar(3)
      ,@BankingDetails ntext
      ,@AccountManagerEmail nvarchar(255)
      ,@Website varchar(255)
)
as
UPDATE CarrierProfile
SET
		
					[Name] = @Name
					,[CompanyName] = @CompanyName
					,[CompanyLogoName] = @CompanyLogoName
					,[Address1]= @Address1
					,[Address2] = @Address2
					,[Address3] = @Address3
					,[Country] = @Country
					,[City] = @City
					,[Telephone] = @Telephone
					,[Fax] = @Fax
					,[BillingContact] = @BillingContact
					,[BillingEmail] = @BillingEmail
					,[PricingContact] = @PricingContact
					,[PricingEmail] = @PricingEmail
					,[SupportContact] = @SupportContact
					,[SupportEmail] = @SupportEmail
					,[CurrencyID] = @CurrencyID
					,[BankingDetails] = @BankingDetails
					,[AccountManagerEmail] = @AccountManagerEmail
					,[Website] = @Website
WHERE ProfileID = @ProfileID