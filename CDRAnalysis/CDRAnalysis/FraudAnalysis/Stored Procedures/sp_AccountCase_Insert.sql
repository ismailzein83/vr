



CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Insert] 
    @AccountNumber varchar(50), 
	@StatusID int,
	@ValidTill DateTime	,
	@UserId int
	
AS
BEGIN
	INSERT INTO [FraudAnalysis].[AccountCase]
           ([AccountNumber]
           ,[StatusID]
           ,[ValidTill]
           ,[UserId])
		VALUES
           (@AccountNumber
           ,@StatusID
           ,@ValidTill
           ,@UserId
           )
	 
END