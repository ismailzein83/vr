

CREATE PROCEDURE [FraudAnalysis].[sp_Dashboard_GetCasesSummary]
(
	@FromDate datetime,
	@ToDate datetime
)
	AS
	BEGIN
		
		select IsNuLL(cs.Name,'Opened') as StatusName, count(st.SubscriberNumber)as CountCases 
		
		from FraudAnalysis.SubscriberThreshold st 
		left join FraudAnalysis.SubscriberCase sc on st.SubscriberNumber = sc.SubscriberNumber 
		left join FraudAnalysis.CaseStatus cs on cs.Id=sc.StatusId
		
		
		where st.DateDay between @FromDate and @ToDate
		group by cs.Name,cs.Id
		
		
	END