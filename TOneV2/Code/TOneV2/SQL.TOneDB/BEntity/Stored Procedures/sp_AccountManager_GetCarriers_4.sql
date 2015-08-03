


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_AccountManager_GetCarriers]
(
	@UserId INT,
	@FromRow INT,
	@ToRow INT
)
AS
BEGIN
SET NOCOUNT ON 

;WITH AllCarrierAccounts AS (
	SELECT ca.CarrierAccountID, cp.Name, ca.NameSuffix, ca.AccountType,
		CASE
			WHEN ca.AccountType in (0, 1) AND AMCust.CarrierAccountID IS NULL
			THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
		END as IsCustomerAvailable,
		CASE
			WHEN ca.AccountType IN (2, 1) AND AMSupp.CarrierAccountID IS NULL
			THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
		END as IsSupplierAvailable,
		ROW_NUMBER() OVER (ORDER BY ca.CarrierAccountId ASC) AS RowNumber
	FROM CarrierAccount ca
	INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
	LEFT JOIN BEntity.AccountManager AMCust ON AMCust.CarrierAccountID = ca.CarrierAccountID and AMCust.RelationType = 1  AND AMCust.UserId != @UserId
	LEFT JOIN BEntity.AccountManager AMSupp ON AMSupp.CarrierAccountID = ca.CarrierAccountID and AMSupp.RelationType = 2 AND AMSupp.UserId != @UserId
	WHERE ca.IsDeleted = 'N' AND ca.ActivationStatus = 2
)

SELECT * FROM AllCarrierAccounts 
WHERE RowNumber between @FromRow AND @ToRow And (IsCustomerAvailable != 0 or IsSupplierAvailable != 0)

SET NOCOUNT OFF
END