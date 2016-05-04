﻿
CREATE PROCEDURE [FraudAnalysis].[sp_CDRDatabase_GetIsReady] 
	@FromTime datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT IsReady FROM FraudAnalysis.CDRDatabase  WITH (NOLOCK)
	WHERE FromTime = @FromTime
    
END