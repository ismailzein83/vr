

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