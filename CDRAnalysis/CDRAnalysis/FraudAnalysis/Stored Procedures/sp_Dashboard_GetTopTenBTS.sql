

CREATE PROCEDURE [FraudAnalysis].[sp_Dashboard_GetTopTenBTS]
(
	@FromDate datetime,
	@ToDate datetime
)
	AS
	BEGIN
		
		select top 10 count(distinct sc.SubscriberNumber) as CountCases, cdr.BTS_id BTS_Id from FraudAnalysis.NormalCDR cdr 
		inner join FraudAnalysis.SubscriberCase sc on cdr.MSISDN=sc.SubscriberNumber
		where cdr.connectdatetime between @FromDate and @ToDate and sc.StatusID = 3
		group by BTS_id
		order by count(distinct sc.SubscriberNumber) desc
		
		
	END