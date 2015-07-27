


CREATE PROCEDURE [FraudAnalysis].[sp_SubscriberCase_Save] 
    @SubscriberNumber varchar(50), 
	@StatusID int,
	@ValidTill DateTime	
AS
BEGIN

SET NOCOUNT ON
	
	UPDATE FraudAnalysis.[SubscriberCase]
    SET StatusID = @StatusID ,ValidTill = @ValidTill
	 WHERE SubscriberNumber = @SubscriberNumber 
	 

	 
	 if (@@rowcount=0)
	 Begin
		INSERT INTO [CDRAnalysisMobile_WF].[FraudAnalysis].[SubscriberCase]
           ([SubscriberNumber]
           ,[StatusID]
           ,[ValidTill])
		VALUES
           (@SubscriberNumber
           ,@StatusID
           ,@ValidTill)
	 End
	 
	 
	 SET NOCOUNT OFF
	 
	 
	 
END