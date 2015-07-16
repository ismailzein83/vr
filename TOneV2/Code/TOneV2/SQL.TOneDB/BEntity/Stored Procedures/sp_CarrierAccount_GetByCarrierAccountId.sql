-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierAccount_GetByCarrierAccountId]
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
	ca.NameSuffix,
	ca.CarrierGroupID,
	cg.CarrierGroupName,
	ca.CarrierGroups
FROM CarrierAccount ca
		INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
		LEFT OUTER JOIN
                   CarrierGroup cg ON ca.CarrierGroupID = cg.CarrierGroupID
                      
			WHERE 
				ca.CarrierAccountID = @CarrierAccountId

END