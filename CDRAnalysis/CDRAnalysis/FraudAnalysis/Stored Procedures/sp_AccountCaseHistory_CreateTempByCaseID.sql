﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCaseHistory_CreateTempByCaseID]
	@TempTableName VARCHAR(200),
	@CaseID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		
		SELECT [ID]
			  ,[CaseID]
			  ,[UserID]
			  ,[Status]
			  ,[StatusTime]
			  ,[Reason]
			
		INTO #RESULT
		
		FROM [FraudAnalysis].[AccountCaseHistory]
		WITH (NOLOCK)
		WHERE CaseID = @CaseID
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
	
	SET NOCOUNT OFF;
END