
CREATE PROCEDURE [BEntity].[sp_CarrierAccount_GetName]
	@CarrierAccountId VARCHAR(5)
AS
BEGIN

SELECT cp.Name, ca.NameSuffix FROM CarrierAccount ca
INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
WHERE ca.CarrierAccountID = @CarrierAccountId

END