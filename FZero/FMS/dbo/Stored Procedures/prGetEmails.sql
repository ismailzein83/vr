CREATE PROCEDURE [dbo].[prGetEmails] (@DestinationEmail varchar(100)='',@FromDate datetime= null,@ToDate datetime=null)
AS

if @ToDate is not null
begin
set @ToDate = DATEADD(DAY, 1, @ToDate) 
end
select mailitem_id Id, recipients [DestinationEmail], subject, 
 send_request_date as CreationDate,
case when sent_status = 'sent' then 'Sent' else 'Outbox' end IsSent, Sent_Date SentDate
from msdb..sysmail_allitems
where recipients LIKE '%'+ @DestinationEmail +'%' 
AND (@ToDate is null OR send_request_date < @ToDate)
 AND (@FromDate is null OR send_request_date >= @FromDate)