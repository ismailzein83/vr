



CREATE PROCEDURE [dbo].[prGetSummaryClient]
(
  @MobileOperatorID int = NULL,
  @FromAttemptDateTime DATETIME = NULL,
  @ToAttemptDateTime DATETIME = NULL,
  @ClientID int = NULL
)
AS
BEGIN  

select m.ID as MobileOperatorID, r.ClientID,  u.FullName,  (select COUNT(*) from GeneratedCalls gg inner join RecievedCalls rc on gg.ID=rc.GeneratedCallID
 where gg.MobileOperatorID=m.ID and rc.ClientID=r.ClientID and  (@FromAttemptDateTime is null or  gg.AttemptDateTime>=@FromAttemptDateTime ) and ( @ToAttemptDateTime is null or   gg.AttemptDateTime<=@ToAttemptDateTime)) as Attempts
, SUM( CASE WHEN g.StatusID = 2 THEN 1 ELSE 0 END )as Bypass

,Count( Distinct(case when StatusID=2 then r.CLI end )) as DistinctCLI  
from RecievedCalls r inner join GeneratedCalls g on  r.GeneratedCallID=g.ID 
                                     right join MobileOperators m on g.MobileOperatorID=m.ID
                                     right join Users u on m.userID=u.ID
where  (@ClientID =0 or  r.ClientID=@ClientID ) and (@MobileOperatorID =0 or  g.MobileOperatorID=@MobileOperatorID ) and (@FromAttemptDateTime is null or  r.AttemptDateTime>=@FromAttemptDateTime ) and ( @ToAttemptDateTime is null or   r.AttemptDateTime<=@ToAttemptDateTime) 
Group by u.FullName,m.ID, r.ClientID

order by r.ClientID




                    
                      

END