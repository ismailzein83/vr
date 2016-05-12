CREATE PROCEDURE [summarytransformation].[sp_SummaryBatchLock_TryLock]
	@TypeID int,
	@BatchStart datetime,
	@CurrentRuntimeProcessID int,
	@RunningProcessIDs varchar(max)
AS
BEGIN
		
	DECLARE @RunningProcessIDsTable TABLE (ID int)
	IF @RunningProcessIDs IS NOT NULL
	BEGIN
		INSERT INTO @RunningProcessIDsTable (ID)
		select Convert(int, ParsedString) from [summarytransformation].[ParseStringList](@RunningProcessIDs)
	END	

	IF NOT EXISTS (SELECT TOP 1 NULL FROM summarytransformation.SummaryBatchLock WITH(NOLOCK) WHERE TypeID = @TypeID AND BatchStart = @BatchStart)
	BEGIN
		BEGIN TRY
			INSERT INTO summarytransformation.SummaryBatchLock 
			(TypeID, BatchStart) 
			SELECT @TypeID, @BatchStart 
			WHERE NOT EXISTS (SELECT TOP 1 NULL FROM summarytransformation.SummaryBatchLock WHERE TypeID = @TypeID AND BatchStart = @BatchStart)
		END TRY
		BEGIN CATCH
			 
		END CATCH
		
	END
	
	DECLARE @IsLocked bit
	SET @IsLocked = 0--default value
	
	UPDATE summarytransformation.SummaryBatchLock
    SET	
			@IsLocked = 1,
			LockedByProcessID = @CurrentRuntimeProcessID
	WHERE TypeID = @TypeID 
			AND BatchStart = @BatchStart		  
			AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
	
	SELECT @IsLocked
END