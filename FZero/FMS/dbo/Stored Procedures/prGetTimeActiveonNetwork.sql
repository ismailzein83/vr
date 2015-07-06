








	CREATE PROCEDURE [dbo].[prGetTimeActiveonNetwork]
	(
	  @MobileOperatorID int = NULL,
	  @FromAttemptDateTime DATETIME = NULL,
	  @ToAttemptDateTime DATETIME = NULL, 
	  @ClientID int  =NULL
	)
	AS
	BEGIN  


			SELECT 1 as Ordered,  'zero_four' as Period , COUNT(*) as Attempts FROM  GeneratedCalls gg inner join RecievedCalls rc on gg.ID=rc.GeneratedCallID WHERE  StatusID=2 and   (gg.MobileOperatorID=@MobileOperatorID or @MobileOperatorID=0) and  (ClientID=@ClientID or @ClientID=0) and   (gg.AttemptDateTime between @FromAttemptDateTime and @ToAttemptDateTime) and CAST(gg.AttemptDateTime AS TIME) BETWEEN '00:00' and '03:59' union 
			SELECT 2 as Ordered,  'four_eight'  as Period , COUNT(*) as Attempts from GeneratedCalls gg inner join RecievedCalls rc on gg.ID=rc.GeneratedCallID WHERE  StatusID=2 and     (gg.MobileOperatorID=@MobileOperatorID or @MobileOperatorID=0) and  (ClientID=@ClientID or @ClientID=0) and   (gg.AttemptDateTime between @FromAttemptDateTime and @ToAttemptDateTime) and CAST(gg.AttemptDateTime AS TIME) BETWEEN '04:00' and '07:59'  union 
			SELECT 3 as Ordered,  'eight_twelve'  as Period , COUNT(*) as Attempts from GeneratedCalls gg inner join RecievedCalls rc on gg.ID=rc.GeneratedCallID WHERE  StatusID=2 and     (gg.MobileOperatorID=@MobileOperatorID or @MobileOperatorID=0) and  (ClientID=@ClientID or @ClientID=0) and  (gg.AttemptDateTime between @FromAttemptDateTime and @ToAttemptDateTime) and CAST(gg.AttemptDateTime AS TIME) BETWEEN '08:00' and '11:59'  union 
			SELECT 4 as Ordered,  'twelve_sixteen'  as Period , COUNT(*) as Attempts from GeneratedCalls gg inner join RecievedCalls rc on gg.ID=rc.GeneratedCallID WHERE  StatusID=2 and     (gg.MobileOperatorID=@MobileOperatorID or @MobileOperatorID=0) and  (ClientID=@ClientID or @ClientID=0)and (gg.AttemptDateTime between @FromAttemptDateTime and @ToAttemptDateTime) and CAST(gg.AttemptDateTime AS TIME) BETWEEN '12:00' and '15:59'  union 
			SELECT 5 as Ordered,  'sixteen_twenty'  as Period , COUNT(*) as Attempts from GeneratedCalls gg inner join RecievedCalls rc on gg.ID=rc.GeneratedCallID WHERE  StatusID=2 and     (gg.MobileOperatorID=@MobileOperatorID or @MobileOperatorID=0) and  (ClientID=@ClientID or @ClientID=0) and (gg.AttemptDateTime between @FromAttemptDateTime and @ToAttemptDateTime) and CAST(gg.AttemptDateTime AS TIME) BETWEEN '16:00' and '19:59'  union   
			SELECT 6 as Ordered,  'twenty_twentyfour'  as Period , COUNT(*) as Attempts from GeneratedCalls gg inner join RecievedCalls rc on gg.ID=rc.GeneratedCallID WHERE  StatusID=2 and     (gg.MobileOperatorID=@MobileOperatorID or @MobileOperatorID=0) and  (ClientID=@ClientID or @ClientID=0)and  (gg.AttemptDateTime between @FromAttemptDateTime and @ToAttemptDateTime) and CAST(gg.AttemptDateTime AS TIME) BETWEEN '20:00' and '23:59'   

			        
	                      

	END