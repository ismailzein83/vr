
CREATE PROCEDURE [FraudAnalysis].[sp_CDRDatabase_UnLock] 
	@FromTime datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE FraudAnalysis.CDRDatabase
	SET LockedByProcessID = NULL
	WHERE FromTime = @FromTime    
END