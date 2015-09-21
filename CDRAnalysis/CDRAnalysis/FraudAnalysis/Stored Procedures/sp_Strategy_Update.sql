

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_Update] 
    @Id int, 
	@UserId int,
	@Name varchar(20),
	@Description varchar(255),
	@LastUpdatedOn DateTime,
	@IsDefault bit,
	@IsEnabled bit,
	@PeriodId int,
	@StrategyContent Nvarchar(max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM FraudAnalysis.[Strategy] WHERE ID != @Id AND Name = @Name)
	BEGIN
		UPDATE FraudAnalysis.[Strategy]
		SET Description = @Description
		  ,UserId = @UserId
		  ,LastUpdatedOn = @LastUpdatedOn
		  ,Name = @Name
		  ,IsDefault = @IsDefault
		  ,IsEnabled = @IsEnabled
		  ,StrategyContent = @StrategyContent
		  ,PeriodId=@PeriodId
		 WHERE Id = @Id
	END
END