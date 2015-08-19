CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_UpdateAccountCases]
(
	@AccountCase As [FraudAnalysis].[AccountCaseType] Readonly 
)
	AS
	BEGIN
		SET NOCOUNT ON
	
	-- Re-Open Pending Cases
		
          declare @CurrentDate datetime; 
          set @CurrentDate=Getdate(); 
            
        INSERT INTO [FraudAnalysis].AccountCase(AccountNumber, StatusId, StrategyId, SuspicionLevelID) 
            SELECT act.AccountNumber, 1, act.StrategyId, act.SuspicionLevelID FROM [FraudAnalysis].AccountCase ac, @AccountCase act 
            WHERE ac.AccountNumber=act.AccountNumber and (ac.StrategyId<>act.StrategyId or ac. SuspicionLevelID<>act.SuspicionLevelID) and  ((ac.StatusID=4  and ac.ValidTill <=@CurrentDate) or (ac.StatusId <>4)  ) and ac.Id =(select max(Id) from  FraudAnalysis.AccountCase acTemp where  acTemp.AccountNumber=act.AccountNumber  )  ;    
		
		
		INSERT INTO [FraudAnalysis].AccountCase(AccountNumber, StatusId, StrategyId, SuspicionLevelID)
		  SELECT act.AccountNumber, 1, act.StrategyId, act.SuspicionLevelID FROM @AccountCase act 
		

		
		
		--INSERT INTO [FraudAnalysis].AccountCase(AccountNumber, StatusId, StrategyId, SuspicionLevelID)
		--  SELECT act.AccountNumber, 1, act.StrategyId, act.SuspicionLevelID FROM @AccountCase act 
		-- left join [FraudAnalysis].AccountCase acTemp ON acTemp.AccountNumber = act.AccountNumber WHERE acTemp.AccountNumber IS NULL

		 
		
		SET NOCOUNT OFF
	END