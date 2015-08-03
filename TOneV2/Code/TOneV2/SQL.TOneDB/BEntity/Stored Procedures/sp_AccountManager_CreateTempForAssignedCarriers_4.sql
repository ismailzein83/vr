
CREATE PROCEDURE [BEntity].[sp_AccountManager_CreateTempForAssignedCarriers]
	@TempTableName VARCHAR(200) = NULL,
	@UserIds [BEntity].[MemberIdType] READONLY,
	@CarrierType SMALLINT
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT am.UserId, cp.Name AS CarrierName, ca.NameSuffix, am.CarrierAccountId, am.RelationType
			INTO #RESULT
			FROM BEntity.AccountManager am
			INNER JOIN dbo.CarrierAccount ca ON am.CarrierAccountId = ca.CarrierAccountID
			INNER JOIN dbo.CarrierProfile cp ON ca.ProfileID = cp.ProfileID
			WHERE am.UserId IN (SELECT Id FROM @UserIds) AND (@CarrierType = 0 OR am.RelationType = @CarrierType)
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END