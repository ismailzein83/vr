
--update normalCDR set isrepeated= null

CREATE PROCEDURE [dbo].[prUpdateRepeatedCalls]
AS

update NormalCDR set IsRepeated=null

select * into #SwitchJadriahTimeDiff from  SwitchJadriahTimeDiff


update a
set a.isrepeated=0 
from  normalCDR a 
where not exists 
(
	select top 1 b.id from normalCDR b where 
	datediff(minute,a.connectdatetime,b.connectdatetime)<
	 (
	      --dbo.FindSwitchesTimeDiff(a.SwitchId,b.SwitchId)
		select 
		case when a.MinutesDiff > b.MinutesDiff then ABS( a.MinutesDiff - b.MinutesDiff ) else ABS( b.MinutesDiff - a.MinutesDiff ) end
		from
		(
		 select MinutesDiff from #SwitchJadriahTimeDiff where SwitchId=a.SwitchId
		)a
		,
		(
		 select MinutesDiff from #SwitchJadriahTimeDiff where SwitchId=b.SwitchId
		)b
		
		
	 )
	and 
	datediff(minute,a.disconnectdatetime,b.disconnectdatetime)<
	(
	 --dbo.FindSwitchesTimeDiff(a.SwitchId,b.SwitchId)
	   select 
		case when a.MinutesDiff > b.MinutesDiff then ABS( a.MinutesDiff - b.MinutesDiff ) else ABS( b.MinutesDiff - a.MinutesDiff ) end
		from
		(
		 select MinutesDiff from #SwitchJadriahTimeDiff where SwitchId=a.SwitchId
		)a
		,
		(
		 select MinutesDiff from #SwitchJadriahTimeDiff where SwitchId=b.SwitchId
		)b
	)
	and  abs(a.durationinseconds - b.durationinseconds )<1
	and a.SwitchId<>b.SwitchId 
	and a.a_temp=b.a_temp
	and a.b_temp=b.b_temp
    and b.id < a.id
)
and a.isrepeated is null
update normalCDR set isrepeated=1 where isrepeated is null

/*

.
*/