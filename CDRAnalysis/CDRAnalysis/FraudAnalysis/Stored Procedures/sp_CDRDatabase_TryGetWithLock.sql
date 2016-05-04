
CREATE PROCEDURE [FraudAnalysis].[sp_CDRDatabase_TryGetWithLock] 
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
	
	DECLARE @IsLocked bit
	UPDATE FraudAnalysis.CDRDatabase
	SET LockedByProcessID = @CurrentRuntimeProcessID,
		@IsLocked = 1
	WHERE FromTime = @FromTime
			AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
    
    SELECT [FromTime]
      ,[ToTime]
      ,[Settings]
      ,[CreatedTime]
      ,[timestamp]
    FROM FraudAnalysis.CDRDatabase
    WHERE FromTime = @FromTime AND @IsLocked = 1
    
END