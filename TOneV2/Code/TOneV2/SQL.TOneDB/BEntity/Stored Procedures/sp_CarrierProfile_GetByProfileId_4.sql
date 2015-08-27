-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierProfile_GetByProfileId]
		@ProfileID INT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	WITH profileAccountsCount AS (SELECT ca.ProfileID, COUNT(*) AccountsCount
										FROM CarrierAccount ca
										GROUP BY ca.ProfileID)
	SELECT cp.ProfileID, 
				cp.Name, 
				cp.CompanyName, 
				cp.BillingEmail,
				cp.Country,
				cp.City,
				cp.RegistrationNumber,
				cp.Telephone,
				cp.Fax,
				cp.Address1,
				cp.Address2,
				cp.Address3,
				cp.Website,
				cp.BillingContact,
				cp.BillingDisputeEmail,
				cp.PricingContact,
				cp.PricingEmail,
				cp.AccountManagerEmail,
				cp.AccountManagerContact,
				cp.SupportContact,
				cp.SupportEmail,
				cp.TechnicalContact,
				cp.TechnicalEmail,
				cp.CommercialContact,
				cp.CommercialEmail,
				cp.SMSPhoneNumber,
				ca.AccountsCount,
				cp.FileID
	FROM CarrierProfile cp
	LEFT JOIN profileAccountsCount ca on ca.ProfileID = cp.ProfileID
	WHERE cp.ProfileID = @ProfileID
END