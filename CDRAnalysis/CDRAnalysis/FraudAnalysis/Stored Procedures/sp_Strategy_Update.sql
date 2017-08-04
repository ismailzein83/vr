

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_Update] 
    @Id int, 
	@UserId int,
	@Name varchar(20),
	@Description varchar(255),
	@LastUpdatedOn DateTime,
	@Settings Nvarchar(max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM FraudAnalysis.[Strategy] WHERE ID != @Id AND Name = @Name)
	BEGIN
		UPDATE FraudAnalysis.[Strategy]
		SET Description = @Description
		  ,UserId = @UserId
		  ,LastUpdatedOn = @LastUpdatedOn
		  ,Name = @Name
		  ,Settings = @Settings
		 WHERE Id = @Id
	END
END