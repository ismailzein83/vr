CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_Insert] 
	@UserId int,
	@Name varchar(20),
	@Description varchar(255),
	@CreationDate DateTime,
	@IsDefault bit,
	@StrategyContent Nvarchar(max),
	@Id int out
	
AS
BEGIN
	IF NOT Exists (SELECT null FROM [dbo].[Strategy] WHERE Name = @Name)
	BEGIN
		Insert into [dbo].[Strategy] ([Description]  ,[UserId]  ,[CreationDate], [Name]  ,[IsDefault]  ,[StrategyContent])
		values(@Description  ,@UserId  ,@CreationDate, @Name  ,@IsDefault  ,@StrategyContent)
	
		SET @Id = @@IDENTITY
	END
END