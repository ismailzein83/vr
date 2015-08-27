

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [BEntity].[sp_CarrierAccount_GetAll]
	@CarrierType int = NULL
AS

SELECT ca.CarrierAccountId,
			cp.ProfileId ,
			cp.Name AS ProfileName,
			cp.CompanyName AS ProfileCompanyName,
			ca.ActivationStatus,
			ca.RoutingStatus,
			ca.AccountType,
			ca.CustomerPaymentType,
			ca.SupplierPaymentType,
			ca.NameSuffix

			FROM CarrierAccount ca
			INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
			
			WHERE ca.IsDeleted = 'N' AND ca.ActivationStatus = 2 AND ca.AccountType IN (@CarrierType,1)
			ORDER BY ProfileName ASC