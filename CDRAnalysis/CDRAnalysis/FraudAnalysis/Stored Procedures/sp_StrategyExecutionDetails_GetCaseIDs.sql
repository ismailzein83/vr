




	CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_GetCaseIDs] 
	
	@AccountNumber VARCHAR(50),
	@FromDate DATETIME,
	@ToDate DATETIME,
	@StrategyIDs varchar(1000)
	
	AS
	BEGIN
	
	    
	    IF (@StrategyIDs IS NOT NULL)
			BEGIN
				DECLARE @StrategyIDsTable TABLE (StrategyID INT);
				INSERT INTO @StrategyIDsTable (StrategyID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@StrategyIDs);
			END
	

		Select distinct CaseID 
		from CDRAnalysisMobile_Wf.FraudAnalysis.StrategyExecutionDetails details
		inner join CDRAnalysisMobile_Wf.FraudAnalysis.StrategyExecution exe on details.StrategyExecutionID=exe.ID
		where 
		--exe.StrategyID in (SELECT StrategyID FROM @StrategyIDsTable) and  
		(@FromDate is null or exe.FromDate >= @FromDate)
		and (@ToDate is null or exe.ToDate <= @ToDate)
		and (@AccountNumber is null or details.AccountNumber = @AccountNumber)
		
		
	  
	END