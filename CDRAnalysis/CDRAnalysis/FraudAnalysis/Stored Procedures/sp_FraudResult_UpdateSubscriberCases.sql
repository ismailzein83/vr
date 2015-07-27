


CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_UpdateSubscriberCases]
(
	@SubscriberNumbersList varchar(max) = ''
)
	AS
	BEGIN
		SET NOCOUNT ON
	
		EXEC('Update [FraudAnalysis].SubscriberCase set StatusID=1 where StatusID =2 and SubscriberNumber IN ('+@SubscriberNumbersList+')')
		
		SET NOCOUNT OFF
	END