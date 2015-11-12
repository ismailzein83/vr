-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [StatisticManagement].[sp_StatisticBatch_TryLock]
	@TypeID int,
	@BatchStart datetime,
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
		select Convert(int, ParsedString) from [StatisticManagement].[ParseStringList](@RunningProcessIDs)
	END	

    INSERT INTO StatisticManagement.StatisticBatch WITH (TABLOCK) 
	(TypeID, BatchStart) 
	SELECT @TypeID, @BatchStart 
	WHERE NOT EXISTS (SELECT TOP 1 NULL FROM StatisticManagement.StatisticBatch WHERE TypeID = @TypeID AND BatchStart = @BatchStart)
	
	DECLARE @IsLocked bit, @IsInfoCorrupted bit, @BatchInfo nvarchar(max)
	
	UPDATE StatisticManagement.StatisticBatch
    SET	
			@IsLocked = 1,
			@IsInfoCorrupted = CASE WHEN LockedByProcessID IS NOT NULL THEN 1 ELSE 0 END,
			LockedByProcessID = @CurrentRuntimeProcessID,
			@BatchInfo = BatchInfo
	WHERE TypeID = @TypeID 
			AND BatchStart = @BatchStart		  
			AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
	
	SELECT @IsInfoCorrupted IsInfoCorrupted, @BatchInfo BatchInfo WHERE @IsLocked = 1
END