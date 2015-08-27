-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierMask_GetByMaskId]
		@MaskID INT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT cm.ID
		  ,cm.Name
		  ,cm.CompanyName
		  ,cm.CountryId
		  ,cm.RegistrationNumber
		  ,cm.VatID
		  ,cm.Telephone1
		  ,cm.Telephone2
		  ,cm.Telephone3
		  ,cm.Fax1
		  ,cm.Fax2
		  ,cm.Fax3
		  ,cm.Address1
		  ,cm.Address2
		  ,cm.Address3
		  ,cm.CompanyLogo
		  ,cm.IsBankReferences
		  ,cm.BillingContact
		  ,cm.BillingEmail
		  ,cm.PricingContact
		  ,cm.PricingEmail
		  ,cm.AccountManagerEmail
		  ,cm.SupportContact
		  ,cm.SupportEmail
		  ,cm.CurrencyId
		  ,cm.PriceList
		  ,cm.MaskInvoiceformat
		  ,cm.MaskOverAllCounter
		  ,cm.YearlyMaskOverAllCounter
	FROM BEntity.CarrierMask cm
	WHERE cm.ID = @MaskID
END