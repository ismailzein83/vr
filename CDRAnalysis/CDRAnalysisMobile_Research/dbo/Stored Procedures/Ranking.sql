CREATE PROCEDURE [dbo].[Ranking]
	 (
	 @FromDate Datetime,
	 @ToDate Datetime
	 )
AS

select distinct st.SubscriberNumber,st.SuspectionLevelId,nc.IMEI into #temp from SubscriberThresholds st
 inner join NormalCDR nc on st.SubscriberNumber=nc.MSISDN

where st.DateDay between @FromDate and @ToDate
order by  st.SubscriberNumber


select * from #temp

/*

Ranking '03/13/2015','03/15/2015'

*/