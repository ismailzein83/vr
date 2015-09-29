
CREATE PROCEDURE [BEntity].[sp_AccountManager_CreateTempByFiltered]
	@TempTableName VARCHAR(200),
	@ManagerId INT,
	@UserIds [BEntity].[MemberIdType] READONLY
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		WITH
		AssignedCustomers
			AS (SELECT DISTINCT CarrierAccountId, UserId FROM BEntity.AccountManager WHERE UserId IN (SELECT Id FROM @UserIds) AND RelationType = 0),
		AssignedSuppliers
			AS (SELECT DISTINCT CarrierAccountId, UserId FROM BEntity.AccountManager WHERE UserId IN (SELECT Id FROM @UserIds) AND RelationType = 2)

		SELECT ca.CarrierAccountID, cp.Name, ca.NameSuffix, ca.AccountType,
			CASE
				WHEN ca.AccountType IN (0, 1) AND AMCust.CarrierAccountID IS NOT NULL
				THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
			END AS IsCustomerAssigned,
			CASE
				WHEN ca.AccountType IN (2, 1) AND AMSupp.CarrierAccountID IS NOT NULL
				THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
			END AS IsSupplierAssigned,
			CASE
				WHEN AMCust.UserId != @ManagerId
				THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsCustomerIndirect,
			CASE
				WHEN AMSupp.UserId != @ManagerId
				THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsSupplierIndirect
		
		INTO #RESULT
		
		FROM CarrierAccount ca
		INNER JOIN CarrierProfile cp ON ca.ProfileID = cp.ProfileID
		LEFT JOIN AssignedCustomers AMCust ON AMCust.CarrierAccountID = ca.CarrierAccountID
		LEFT JOIN AssignedSuppliers AMSupp ON AMSupp.CarrierAccountID = ca.CarrierAccountID

		WHERE ca.IsDeleted = 'N'
			  AND ca.ActivationStatus = 2
			  AND (AMCust.CarrierAccountId IS NOT NULL OR AMSupp.CarrierAccountId IS NOT NULL)


		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END

	SET NOCOUNT OFF
END