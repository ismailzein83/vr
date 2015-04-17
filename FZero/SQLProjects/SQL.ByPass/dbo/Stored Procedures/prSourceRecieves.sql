

CREATE PROCEDURE [dbo].[prSourceRecieves]
(
 @StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
)
AS
BEGIN    


SELECT     TOP (100) PERCENT COUNT(RecievedCalls.ID) AS CountRecieved, DATEADD(dd, 0, DATEDIFF(dd, 0, RecievedCalls.AttemptDateTime)) AS attemptdate, Sources.Name, 
                      Clients.Name AS ClientName
FROM         Sources INNER JOIN
                      RecievedCalls ON Sources.ID = RecievedCalls.SourceID INNER JOIN
                      Clients ON RecievedCalls.ClientID = Clients.ID
WHERE     (RecievedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate)
GROUP BY DATEADD(dd, 0, DATEDIFF(dd, 0, RecievedCalls.AttemptDateTime)), Sources.Name, Clients.Name
ORDER BY attemptdate DESC, Sources.Name DESC

END