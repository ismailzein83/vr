


CREATE PROCEDURE [dbo].[prReports]
(
 @StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
)
AS
BEGIN    

SELECT     Users.FullName, Reports.ReportID, Reports.SentDateTime, COUNT(GeneratedCalls.ID) AS Cases, Clients.Name AS ClientName
FROM         Reports INNER JOIN
                      GeneratedCalls ON Reports.ID = GeneratedCalls.ReportID INNER JOIN
                      RecievedCalls on RecievedCalls.GeneratedCallID=GeneratedCalls.ID Inner Join
                      MobileOperators ON RecievedCalls.MobileOperatorID = MobileOperators.ID INNER JOIN
                      Users ON MobileOperators.UserID = Users.ID INNER JOIN
                      Clients ON RecievedCalls.ClientID = Clients.ID
WHERE     (Reports.SentDateTime BETWEEN @StartDate AND @EndDate)
GROUP BY Reports.ReportID, Reports.SentDateTime, Users.FullName, Clients.Name
ORDER BY Reports.SentDateTime DESC


END