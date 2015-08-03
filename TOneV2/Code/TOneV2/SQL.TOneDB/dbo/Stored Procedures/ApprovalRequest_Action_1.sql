
CREATE PROCEDURE [dbo].[ApprovalRequest_Action] 
AS
BEGIN
Declare  @timeout decimal  --in minute as (from hour timeout or days timeout
Declare  @timeoutaction decimal 
declare @timeoutdays nvarchar(20)
declare @timeoutdaysinhour int --from day and hour to minute
Select @timeoutdays=s.TimeSpanValue from SystemParameter s Where Name like 'ApprovalRequest_TimeOut'	
declare @hour int=0
declare @day int=0
declare @minute int=0

declare @part varchar(5)
declare @dayhour int=0


select @part=  SUBSTRING(@timeoutdays, 0, PATINDEX('%:%',@timeoutdays));
select @timeoutdays=SUBSTRING(@timeoutdays, PATINDEX('%:%',@timeoutdays)+1, len(@timeoutdays))

if PATINDEX('%.%',@part)>0
begin
	select @day=CAST(SUBSTRING(@part, 0, PATINDEX('%.%',@part)) as int);
	select @part=SUBSTRING(@part, PATINDEX('%.%',@part)+1, len(@part)+1)
	select @dayhour =CAST(@part as int)
end
else
begin 
	select @day=  CAST( SUBSTRING(@part, 0, PATINDEX('%:%',@part)) as int)
end 


select @hour=CAST(SUBSTRING(@timeoutdays, 0, PATINDEX('%:%',@timeoutdays))  as int)
select @timeoutdays=SUBSTRING(@timeoutdays, PATINDEX('%:%',@timeoutdays)+1, len(@timeoutdays))
select @minute= CAST( @timeoutdays as int)
select @timeoutdaysinhour=(@day*24*60) +(@dayhour*60) + (@hour*60) +@minute;

if @timeoutdaysinhour=0 --set to null if is zero 
	set @timeoutdaysinhour=null;
	
Select	@timeout=isnull(@timeoutdaysinhour,(isnull(NumericValue,1)*60)) 
from	SystemParameter 
Where	Name like 'ApprovalRequest_TimeOutActionHours'

declare @ApprovalIDs  table(id int);

Select	@timeoutaction=NumericValue 
from	SystemParameter 
Where	Name like 'ApprovalRequest_TimeOutAction'

if(@timeoutaction=1)
begin
	insert into @ApprovalIDs
	select	r.ID
	from	ApprovalRequest r 
			inner join AMU a on r.OriginatorAmuID=a.ID 
			inner join RateRequest rr on rr.OwnerApprovalRequestID=r.ID
	where	rr.RateRequestStatus=5 and DATEDIFF(MINUTE,ISNULL(r.Lasttimeoutactiondate,r.Date),GETDATE())>@timeout and a.ParentID>0

	update  r  
	set		r.AssignedToAmuID=isnull((select ParentID from AMU where ID= case when r.AssignedToAmuID is null then r.OriginatorAmuID else r.AssignedToAmuID end),r.AssignedToAmuID)  ,
			r.Note = case when  (r.AssignedToAmuID = isnull((select ParentID from AMU where ID= case when r.AssignedToAmuID is null then r.OriginatorAmuID else r.AssignedToAmuID end),r.AssignedToAmuID)) then 'Time out extended due to Escalation which is Reach the top level AMU  as timeout action' else 'Forwarded to Higer level as timeout action'  end ,
			r.ForwarderAmuID=a.ID,
			r.Lasttimeoutactiondate=getdate()
	from	ApprovalRequest r 
			inner join AMU a on r.OriginatorAmuID=a.ID
			inner join RateRequest rr on rr.OwnerApprovalRequestID=r.ID
	where	rr.RateRequestStatus=5 and DATEDIFF(MINUTE,ISNULL(r.Lasttimeoutactiondate,r.Date) ,GETDATE())>@timeout and a.ParentID>0

end
else
begin
	insert into @ApprovalIDs
	select	r.ID
	from	ApprovalRequest r 
			inner join AMU a on r.OriginatorAmuID=a.ID 
			inner join RateRequest rr on rr.OwnerApprovalRequestID=r.ID
	where	rr.RateRequestStatus=5 and DATEDIFF(MINUTE,ISNULL(r.Lasttimeoutactiondate,r.Date),GETDATE())>@timeout and a.ParentID>0

	update	r
	set		r.Note='Approval Request Time Out Extended as time out action',
			r.Lasttimeoutactiondate=getdate()
	from	ApprovalRequest r 
			inner join AMU a on r.OriginatorAmuID=a.ID
			inner join RateRequest rr on rr.OwnerApprovalRequestID=r.ID
	where	rr.RateRequestStatus=5 and DATEDIFF(MINUTE,ISNULL(r.Lasttimeoutactiondate,r.Date),GETDATE())>@timeout and a.ParentID>0
end

select * from @ApprovalIDs; 
select @timeout+1

END