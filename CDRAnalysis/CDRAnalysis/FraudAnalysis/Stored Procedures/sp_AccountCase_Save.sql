



CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Save] 
    @AccountNumber varchar(50), 
	@StatusID int,
	@ValidTill DateTime	
AS
BEGIN

SET NOCOUNT ON
	
	UPDATE FraudAnalysis.[AccountCase]
    SET StatusID = @StatusID ,ValidTill = @ValidTill
	 WHERE AccountNumber = @AccountNumber 
	 

	 
	 if (@@rowcount=0)
	 Begin
		INSERT INTO [FraudAnalysis].[AccountCase]
           ([AccountNumber]
           ,[StatusID]
           ,[ValidTill])
		VALUES
           (@AccountNumber
           ,@StatusID
           ,@ValidTill)
	 End
	 
	 
	 SET NOCOUNT OFF
	 
	 
	 
END