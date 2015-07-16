-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[SP_Carriers_GetAllCarriers]
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
	ca.CarrierGroups,
	ca.CarrierGroupID
FROM
	CarrierAccount ca
	INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
			
where ca.IsDeleted = 'N' --AND ca.ActivationStatus = 2
END