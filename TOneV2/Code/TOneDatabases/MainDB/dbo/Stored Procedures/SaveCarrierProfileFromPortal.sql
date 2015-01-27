
CREATE Proc [dbo].[SaveCarrierProfileFromPortal]
(
		
      @Name nvarchar(200)=null
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
	  ,@CommercialContact nvarchar(200)
	  ,@CommercialEmail nvarchar(256)
      ,@CurrencyID varchar(3)
      ,@BankingDetails ntext
      ,@AccountManagerEmail nvarchar(255)
      ,@Website varchar(255)
)
as

BEGIN
	DECLARE @tempCurrencyID VARCHAR(3) SET @tempCurrencyID = @CurrencyID
		IF not EXISTS (SELECT c.CurrencyID FROM Currency c WHERE c.CurrencyID = @CurrencyID)
		BEGIN
			
		SELECT @tempCurrencyID = c.CurrencyID
		  FROM Currency c WHERE c.IsMainCurrency = 'Y'
			END			
			
			insert into [CarrierProfile] ([Name],[CompanyName],[CompanyLogoName],[Address1],[Address2],[Address3],[Country],[City],[Telephone],[Fax],[BillingContact],[BillingEmail],[PricingContact],[PricingEmail],[SupportContact],[SupportEmail],[CurrencyID],[BankingDetails],[AccountManagerEmail],[Website],[IsDeleted] ,UserID,[CommercialContact],[CommercialEmail])
					values(@Name,@CompanyName,@CompanyLogoName,@Address1,@Address2,@Address3,@Country,@City,@Telephone,@Fax,@BillingContact,@BillingEmail,@PricingContact,@PricingEmail,@SupportContact,@SupportEmail,@tempCurrencyID,@BankingDetails,@AccountManagerEmail,@Website,'N',NULL,@CommercialContact,@CommercialEmail)
					return SCOPE_IDENTITY()--Insertion done succsessful
		
 
END