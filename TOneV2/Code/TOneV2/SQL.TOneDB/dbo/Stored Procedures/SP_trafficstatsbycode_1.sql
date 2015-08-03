
CREATE PROCEDURE [dbo].[SP_trafficstatsbycode]	
	
AS
declare @fromdate datetime
declare @numberofdays int
declare @todate datetime

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements. 	TrafficStatsByCode
	SET NOCOUNT ON;
	
    select @fromdate = dateadd(DAY,-1, datediff(DAY,0,getdate()))
    set @todate = dateadd(dd,2 ,@fromdate)

delete  TrafficStatsByCode 
from	TrafficStatsByCode with(index=IX_TrafficByCode_FirstCDRAttempt)
where	firstCDRattempt between @fromdate and @todate


END