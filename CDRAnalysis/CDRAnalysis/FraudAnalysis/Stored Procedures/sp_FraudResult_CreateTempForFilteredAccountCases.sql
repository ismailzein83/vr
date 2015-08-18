

CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_CreateTempForFilteredAccountCases]
(
	@TempTableName varchar(200),	
	@AccountNumber varCHAR(100)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	    
	      SELECT ac.[ID] ,ac.[AccountNumber] ,ac.[StatusID], cs.Name StatusName ,  s. Name as StrategyName ,  ac.[ValidTill] ,ac.[StrategyId] ,ac.[UserId] ,ac.[LogDate] , ac.SuspicionLevelID, sl.Name as SuspicionLevelName
	      into #Result
	      FROM [FraudAnalysis].[AccountCase] as ac
	      left join [FraudAnalysis].[Strategy] s on s.id = ac.StrategyId
	      inner join [FraudAnalysis].[CaseStatus] cs on ac.StatusID = cs.Id
	      left join FraudAnalysis.SuspicionLevel sl on sl.Id = ac.SuspicionLevelID
	      where ac.[AccountNumber]=@AccountNumber 
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END