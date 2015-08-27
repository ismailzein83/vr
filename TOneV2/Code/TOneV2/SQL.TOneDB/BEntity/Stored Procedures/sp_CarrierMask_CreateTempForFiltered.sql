-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [BEntity].[sp_CarrierMask_CreateTempForFiltered]
	@TempTableName varchar(200),
	@Name nvarchar(200) =  NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		
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
			INTO #RESULT
			FROM [BEntity].[CarrierMask] cm WITH(NOLOCK)
			WHERE(@Name IS NULL OR cm.Name like '%' + @Name + '%')
			ORDER BY cm.Name DESC
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

END