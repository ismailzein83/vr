







CREATE  PROCEDURE [dbo].[prSummary]
(
@MobileOperatorID INT = NULL
,@StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
, @ClientID int = NULL
)
AS
BEGIN    






	SELECT    1 as ordered,   'Successful Calls' as CallType,  COUNT(RecievedCalls.ID) AS count  FROM         RecievedCalls inner join GeneratedCalls on RecievedCalls.GeneratedCallID=GeneratedCalls.ID
	WHERE     (@MobileOperatorID=0 or dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID ) AND   (@ClientID=0 or dbo.RecievedCalls.ClientID=@ClientID) AND (RecievedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate)

union 


	SELECT    2 as ordered,   'Bypass Calls' as CallType,   COUNT( RecievedCalls.CLI) AS count  FROM         RecievedCalls inner join 
						 GeneratedCalls  ON  RecievedCalls.GeneratedCallID =GeneratedCalls.ID

	WHERE     (@MobileOperatorID=0 or dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID )  AND   (@ClientID=0 or dbo.RecievedCalls.ClientID=@ClientID) AND (GeneratedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate) AND (StatusID =2)

union 

	SELECT     3 as ordered,   'Distinct Bypass ' as CallType, COUNT(distinct RecievedCalls.CLI) AS count  FROM         RecievedCalls inner join 
						 GeneratedCalls  ON  RecievedCalls.GeneratedCallID =GeneratedCalls.ID

	WHERE     (@MobileOperatorID=0 or dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID )  AND   (@ClientID=0 or dbo.RecievedCalls.ClientID=@ClientID) AND (GeneratedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate) AND (StatusID =2)
	
union 
	
	SELECT     4 as ordered,   'Onnet Bypass ' as CallType, Count(distinct RecievedCalls.CLI) AS count
	FROM         GeneratedCalls LEFT OUTER JOIN
						  RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID
	WHERE     (@MobileOperatorID=0 or dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID )  AND  dbo.RecievedCalls.MobileOperatorID=dbo.GeneratedCalls.MobileOperatorID  AND  (@ClientID=0 or dbo.RecievedCalls.ClientID=@ClientID)  AND (GeneratedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate) AND (StatusID =2)
	
	
union 
	
	SELECT     5 as ordered,   'Ofnet Bypass ' as CallType, Count(distinct RecievedCalls.CLI) AS count
	FROM         GeneratedCalls LEFT OUTER JOIN
						  RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID
	WHERE      (@MobileOperatorID=0 or dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID )  AND dbo.RecievedCalls.MobileOperatorID<>dbo.GeneratedCalls.MobileOperatorID  AND  (@ClientID=0 or dbo.RecievedCalls.ClientID=@ClientID)  AND (GeneratedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate) AND (StatusID =2)





END