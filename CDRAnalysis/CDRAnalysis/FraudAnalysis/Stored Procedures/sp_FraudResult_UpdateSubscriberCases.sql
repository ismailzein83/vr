


CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_UpdateSubscriberCases]
(
	@SubscriberCase As [FraudAnalysis].[SubscriberCaseType] Readonly
)
	AS
	BEGIN
		SET NOCOUNT ON
	
	-- Re-Open Pending Cases
		
		UPDATE sc SET sc.StatusID=1 FROM [FraudAnalysis].SubscriberCase sc, @SubscriberCase sct 
            WHERE sc.SubscriberNumber=sct.SubscriberNumber and sc.StatusID=2
		
		
		SET NOCOUNT OFF
	END