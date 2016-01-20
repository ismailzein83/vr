-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_CDRDatabase_TryLockIfDBNotReady] 
	@FromTime datetime,
	@CurrentRuntimeProcessID int,
	@RunningProcessIDs varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @RunningProcessIDsTable TABLE (ID int)
	IF @RunningProcessIDs IS NOT NULL
	BEGIN
		INSERT INTO @RunningProcessIDsTable (ID)
		select Convert(int, ParsedString) from [FraudAnalysis].[ParseStringList](@RunningProcessIDs)
	END	
	
	IF NOT EXISTS (SELECT TOP 1 NULL FROM FraudAnalysis.CDRDatabase WITH(NOLOCK) WHERE FromTime = @FromTime)
	BEGIN
		INSERT INTO FraudAnalysis.CDRDatabase --WITH (TABLOCK) 
		(FromTime, CreatedTime) 
		SELECT @FromTime, GETDATE()
		WHERE NOT EXISTS (SELECT TOP 1 NULL FROM FraudAnalysis.CDRDatabase WHERE FromTime = @FromTime)
	END
	
	DECLARE @IsLocked bit
	UPDATE FraudAnalysis.CDRDatabase
	SET LockedByProcessID = @CurrentRuntimeProcessID,
		@IsLocked = 1
	WHERE FromTime = @FromTime
			AND ISNULL(IsReady, 0) = 0
		AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
    
    SELECT ISNULL(@IsLocked, 0)
    
END