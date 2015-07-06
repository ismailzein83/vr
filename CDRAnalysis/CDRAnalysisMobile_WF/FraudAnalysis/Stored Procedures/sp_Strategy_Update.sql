

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_Update] 
    @Id int, 
	@UserId int,
	@Name varchar(20),
	@Description varchar(255),
	@CreationDate DateTime,
	@IsDefault bit,
	@StrategyContent Nvarchar(max)
AS
BEGIN
	UPDATE FraudAnalysis.[Strategy]
    SET Description = @Description
      ,UserId = @UserId
      ,CreationDate = @CreationDate
      ,Name = @Name
      ,IsDefault = @IsDefault
      ,StrategyContent = @StrategyContent
	 WHERE Id = @Id
END