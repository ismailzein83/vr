



CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Insert] 
    @AccountNumber varchar(50), 
	@StatusID int,
	@ValidTill DateTime	,
	@UserId int,
	@StrategyId int, 
	@SuspicionLevelID int
	
AS
BEGIN
	INSERT INTO [FraudAnalysis].[AccountCase]
           ([AccountNumber]
           ,[StatusID]
           ,[ValidTill]
           ,[UserId]
           ,[StrategyId]
           ,[SuspicionLevelID])
		VALUES
           (@AccountNumber
           ,@StatusID
           ,@ValidTill
           ,@UserId
           ,@StrategyId
           ,@SuspicionLevelID
           )
	 
END