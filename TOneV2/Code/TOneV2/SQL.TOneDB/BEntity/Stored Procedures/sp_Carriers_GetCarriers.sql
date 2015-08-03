

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Carriers_GetCarriers]
	@CarrierType VARCHAR(10) = NULL
AS


IF(@CarrierType IS NULL OR @CarrierType = 'Exchange')
BEGIN
SELECT ca.CarrierAccountID, cp.Name, ca.NameSuffix  FROM CarrierAccount ca
INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
WHERE ca.IsDeleted = 'N' AND ca.ActivationStatus = 2
ORDER BY Name ASC
END
IF(@CarrierType = 'Customer')
BEGIN
SELECT ca.CarrierAccountID, cp.Name, ca.NameSuffix FROM CarrierAccount ca
INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
WHERE ca.IsDeleted = 'N' AND ca.ActivationStatus = 2 AND ca.AccountType IN (0,1)
ORDER BY Name ASC
END
IF(@CarrierType = 'Supplier')
BEGIN
SELECT ca.CarrierAccountID, cp.Name, ca.NameSuffix FROM CarrierAccount ca
INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
WHERE ca.IsDeleted = 'N' AND ca.ActivationStatus = 2 AND ca.AccountType IN (2,1)
ORDER BY Name ASC
END