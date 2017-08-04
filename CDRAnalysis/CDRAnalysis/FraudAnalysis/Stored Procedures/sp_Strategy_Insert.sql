


CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_Insert]
	@UserId int,
	@Name varchar(20),
	@Description varchar(255),
	@LastUpdatedOn DateTime,
	@Settings Nvarchar(max),
	@Id int out
	
AS
BEGIN
	IF NOT Exists (SELECT null FROM FraudAnalysis.[Strategy] WHERE Name = @Name)
	BEGIN
		Insert into FraudAnalysis.[Strategy] ([Description]  ,[UserId]  ,[LastUpdatedOn], [Name]  ,Settings)
		values(@Description  ,@UserId  ,@LastUpdatedOn, @Name  ,@Settings)
	
		SET @Id = @@IDENTITY
	END
END