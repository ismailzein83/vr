-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceState_TryLockAndGet] 
	@DataSourceID uniqueidentifier,
	@CurrentRuntimeProcessID int,	
	@RunningRuntimeProcessIDs VARCHAR(MAX) = NULL
AS
BEGIN
	
	DECLARE @RunningRuntimeProcessIDsTable TABLE (ID int)
	INSERT INTO @RunningRuntimeProcessIDsTable (ID)
	select Convert(int, ParsedString) from [integration].[ParseStringList](@RunningRuntimeProcessIDs)
	
	IF NOT EXISTS (SELECT TOP 1 NULL FROM integration.DataSourceState WHERE DataSourceID = @DataSourceID)
	BEGIN
		INSERT INTO integration.DataSourceState
		(DataSourceID)
		SELECT @DataSourceID WHERE NOT EXISTS (SELECT NULL FROM integration.DataSourceState WHERE DataSourceID = @DataSourceID)
	END
	DECLARE @IsLocked bit
	
	UPDATE integration.DataSourceState
	SET @IsLocked = 1,
		LockedByProcessID = @CurrentRuntimeProcessID
	WHERE DataSourceID = @DataSourceID
		 AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningRuntimeProcessIDsTable))
	
	
	SELECT [DataSourceID]
      ,[State]
    FROM integration.DataSourceState
    WHERE DataSourceID = @DataSourceID AND @IsLocked = 1
	
END