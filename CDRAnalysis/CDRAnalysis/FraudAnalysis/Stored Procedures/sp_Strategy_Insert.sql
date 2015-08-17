


CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_Insert]
	@UserId int,
	@Name varchar(20),
	@Description varchar(255),
	@LastUpdatedOn DateTime,
	@IsDefault bit,
	@IsEnabled bit,
	@PeriodId int,
	@StrategyContent Nvarchar(max),
	@Id int out
	
AS
BEGIN
	IF NOT Exists (SELECT null FROM FraudAnalysis.[Strategy] WHERE Name = @Name)
	BEGIN
		Insert into FraudAnalysis.[Strategy] ([Description]  ,[UserId]  ,[LastUpdatedOn], [Name]  ,[IsDefault] ,[IsEnabled]  ,[StrategyContent], [PeriodId])
		values(@Description  ,@UserId  ,@LastUpdatedOn, @Name  ,@IsDefault ,@IsEnabled  ,@StrategyContent, @PeriodId)
	
		SET @Id = @@IDENTITY
	END
END