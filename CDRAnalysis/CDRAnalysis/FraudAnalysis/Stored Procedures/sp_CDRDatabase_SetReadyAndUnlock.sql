
CREATE PROCEDURE [FraudAnalysis].[sp_CDRDatabase_SetReadyAndUnlock] 
	@FromTime datetime,
	@ToTime datetime,
	@Settings nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE FraudAnalysis.CDRDatabase
	SET IsReady = 1,
		LockedByProcessID = NULL,
		ToTime = @ToTime,
		Settings = @Settings
	WHERE FromTime = @FromTime
    
END