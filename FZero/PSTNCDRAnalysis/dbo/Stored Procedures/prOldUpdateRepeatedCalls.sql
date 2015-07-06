CREATE PROCEDURE [dbo].[prOldUpdateRepeatedCalls]
AS

update a
set a.isrepeated=0 
from  normalCDR a 
where not exists 
(
	select top 1 b.id from normalCDR b where 
	datediff(minute,a.connectdatetime,b.connectdatetime)< dbo.FindSwitchesTimeDiff(a.SwitchId,b.SwitchId)
	and 
	datediff(minute,a.disconnectdatetime,b.disconnectdatetime)< dbo.FindSwitchesTimeDiff(a.SwitchId,b.SwitchId)
	--and CAST(a.durationinseconds as int) =cast(b.durationinseconds as int)
	and  abs(a.durationinseconds - b.durationinseconds )<1
	and a.SWITCh<>b.SWITCh 
	and a.a_temp=b.a_temp
	and a.b_temp=b.b_temp
    and b.id < a.id
)
and a.isrepeated is null
update normalCDR set isrepeated=1 where isrepeated is null

/*

.
*/