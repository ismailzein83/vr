



CREATE PROCEDURE [dbo].[prSourceGeneratesRecieves]
(
 @StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
)
AS
BEGIN    

SELECT     RecievedBy, SentBy, attemptdate, CountGenerated, ClientName
FROM         (SELECT DISTINCT 
                                              TOP (100) PERCENT Sources_1.Name AS RecievedBy, Sources.Name AS SentBy, DATEADD(dd, 0, DATEDIFF(dd, 0, gc.AttemptDateTime)) 
                                              AS attemptdate, COUNT(gc.ID) AS CountGenerated, Clients.Name AS ClientName
                       FROM          GeneratedCalls gc
                        inner join RecievedCalls rc on rc.GeneratedCallID=gc.ID
                       
                        INNER JOIN   Sources AS Sources_1 ON rc.SourceID = Sources_1.ID INNER JOIN
                                              Sources ON gc.SourceID = Sources.ID INNER JOIN
                                              Clients ON rc.ClientID = Clients.ID
                       WHERE      (gc.AttemptDateTime BETWEEN @StartDate AND @EndDate)
                       GROUP BY DATEADD(dd, 0, DATEDIFF(dd, 0, gc.AttemptDateTime)), Sources_1.Name, Sources.Name, Clients.Name) AS s
ORDER BY attemptdate DESC



END