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
        SELECT	distinct act.AccountNumber, 1, act.StrategyId, act.SuspicionLevelID 
        FROM	[FraudAnalysis].AccountCase ac with(nolock,index=IX_AccountCase_AccountNumber)
				, @AccountCase act 
        WHERE	ac.AccountNumber=act.AccountNumber  
				and  ((ac.StatusID=4  and ac.ValidTill <=@CurrentDate) or (ac.StatusId <>4 and (ac.StrategyId<>act.StrategyId or ac. SuspicionLevelID<>act.SuspicionLevelID))  ) 
				and ac.Id =(select max(Id) from  FraudAnalysis.AccountCase acTemp  with(nolock,index=IX_AccountCase_AccountNumber)where  acTemp.AccountNumber=act.AccountNumber  )  ;    
		
		
		--INSERT INTO [FraudAnalysis].AccountCase(AccountNumber, StatusId, StrategyId, SuspicionLevelID)
		--  SELECT act.AccountNumber, 1, act.StrategyId, act.SuspicionLevelID FROM @AccountCase act 
		

		
		
		INSERT INTO [FraudAnalysis].AccountCase(AccountNumber, StatusId, StrategyId, SuspicionLevelID)
		SELECT	distinct act.AccountNumber, 1, act.StrategyId, act.SuspicionLevelID 
		FROM	@AccountCase act 
				left join [FraudAnalysis].AccountCase acTemp with(nolock,index=IX_AccountCase_AccountNumber) ON acTemp.AccountNumber = act.AccountNumber where acTemp.AccountNumber IS NULL

		 
		
		SET NOCOUNT OFF
	END