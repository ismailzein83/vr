


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_AccountManager_GetCarriers]
(
	@UserId INT
)
AS
BEGIN
SET NOCOUNT ON 

;WITH AllCarrierAccounts AS (

	SELECT ca.CarrierAccountID, cp.Name, ca.NameSuffix, ca.AccountType,
		CASE
			WHEN ca.AccountType IN (0, 1) AND AMCust.CarrierAccountID IS NULL
			THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
		END AS IsCustomerAvailable,
		CASE
			WHEN ca.AccountType IN (2, 1) AND AMSupp.CarrierAccountID IS NULL
			THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
		END AS IsSupplierAvailable
	
	FROM CarrierAccount ca
	INNER JOIN CarrierProfile cp ON ca.ProfileID = cp.ProfileID
	LEFT JOIN BEntity.AccountManager AMCust ON AMCust.CarrierAccountID = ca.CarrierAccountID AND AMCust.RelationType = 1 AND AMCust.UserId != @UserId
	LEFT JOIN BEntity.AccountManager AMSupp ON AMSupp.CarrierAccountID = ca.CarrierAccountID AND AMSupp.RelationType = 2 AND AMSupp.UserId != @UserId

	WHERE ca.IsDeleted = 'N' AND ca.ActivationStatus = 2
)

SELECT *
FROM AllCarrierAccounts
WHERE (IsCustomerAvailable != 0 OR IsSupplierAvailable != 0)

SET NOCOUNT OFF
END