


CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_Insert]
	@ProcessID int,
	@StrategyID int,
	@FromDate DateTime,
	@ToDate DateTime,
	@PeriodId int,
	@ExecutionDate DateTime,
	@ExecutedBy int ,
	@Status int, 
	@Id int out
	
AS
BEGIN
		Insert into FraudAnalysis.[StrategyExecution] (ProcessID  ,StrategyID  ,FromDate, ToDate  ,PeriodId ,ExecutionDate, ExecutedBy, [Status])
		values(@ProcessID  ,@StrategyID  ,@FromDate, @ToDate  ,@PeriodId ,@ExecutionDate, @ExecutedBy, @Status )
	
		SET @Id = @@IDENTITY
END