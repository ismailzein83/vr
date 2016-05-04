
CREATE PROCEDURE [FraudAnalysis].[sp_CDRDatabase_UpdateSettingsAndUnLock] 
	@FromTime datetime,
	@Settings nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE FraudAnalysis.CDRDatabase
	SET LockedByProcessID = NULL,
		Settings = @Settings
	WHERE FromTime = @FromTime
    
END