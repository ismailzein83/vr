


CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_Insert]
	@ProcessID int,
	@StrategyID int,
	@FromDate DateTime,
	@ToDate DateTime,
	@PeriodId int,
	@ExecutionDate DateTime,
	@Id int out
	
AS
BEGIN
		Insert into FraudAnalysis.[StrategyExecution] (ProcessID  ,StrategyID  ,FromDate, ToDate  ,PeriodId ,ExecutionDate)
		values(@ProcessID  ,@StrategyID  ,@FromDate, @ToDate  ,@PeriodId ,@ExecutionDate )
	
		SET @Id = @@IDENTITY
END