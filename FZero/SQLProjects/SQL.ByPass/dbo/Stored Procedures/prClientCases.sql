


CREATE PROCEDURE [dbo].[prClientCases]
(
 @StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
)
AS
BEGIN    


SELECT     Clients.Name AS ClientName, COUNT(RecievedCalls.CLI) AS Count, Sources.Name AS GeneratedBy, Sources_1.Name AS ReceivedBy, 
                      Statuses.Name AS Status
FROM         RecievedCalls INNER JOIN
                      GeneratedCalls ON RecievedCalls.GeneratedCallID = GeneratedCalls.ID INNER JOIN
                      Clients ON RecievedCalls.ClientID = Clients.ID INNER JOIN
                      Sources ON GeneratedCalls.SourceID = Sources.ID INNER JOIN
                      Sources AS Sources_1 ON RecievedCalls.SourceID = Sources_1.ID INNER JOIN
                      Statuses ON GeneratedCalls.StatusID = Statuses.ID
WHERE     (GeneratedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate)
GROUP BY Statuses.Name, Clients.Name, Sources.Name, Sources_1.Name

END