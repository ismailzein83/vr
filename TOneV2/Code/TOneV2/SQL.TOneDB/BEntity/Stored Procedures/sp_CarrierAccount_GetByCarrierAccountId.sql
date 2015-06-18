-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [BEntity].[sp_CarrierAccount_GetByCarrierAccountId]
	 (@CarrierAccountId VARCHAR(30) =  NULL)
AS
BEGIN
	SET NOCOUNT ON;

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
			WHERE 
				ca.CarrierAccountID = @CarrierAccountId

END