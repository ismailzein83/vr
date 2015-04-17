















CREATE PROCEDURE [dbo].[prViewOrigination]
(
@MobileOperatorID INT = NULL
,@StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
,@ClientID INT = NULL

)
AS
BEGIN    



--SELECT     distinct (CASE WHEN OriginationNetworks.Name IS NULL 
--                      THEN 'INTERNATIONAL' WHEN OriginationNetworks.Name = '' THEN 'INTERNATIONAL' ELSE 'Zone ' + RIGHT('00' + CONVERT(VARCHAR(3), OriginationNetworks.ID), 3) END) 
--                      AS OriginationNetwork, GeneratedCalls.MobileOperatorID, RecievedCalls.AttemptDateTime, COUNT(*) AS count, SourceKinds.Name AS SourceKind
--FROM         GeneratedCalls INNER JOIN OriginationNetworks
--                       ON OriginationNetworks.Name = GeneratedCalls.OriginationNetwork inner JOIN
--                      RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID inner JOIN
--                      Sources ON RecievedCalls.SourceID = Sources.ID inner JOIN
--                      SourceKinds ON SourceKinds.ID = Sources.SourceKindID
--WHERE  ClientID=@ClientID and           (RecievedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate)
--GROUP BY OriginationNetworks.Name, GeneratedCalls.MobileOperatorID, RecievedCalls.AttemptDateTime, SourceKinds.Name, OriginationNetworks.ID
--ORDER BY OriginationNetwork, SourceKind


SELECT     (CASE WHEN GeneratedCalls.OriginationNetwork IS NULL 
                      THEN 'INTERNATIONAL' WHEN GeneratedCalls.OriginationNetwork = '' THEN 'INTERNATIONAL' ELSE GeneratedCalls.OriginationNetwork END) 
                      AS OriginationNetwork, GeneratedCalls.MobileOperatorID, Max(RecievedCalls.AttemptDateTime) as AttemptDateTime, COUNT(*) AS count, SourceKinds.Name AS SourceKind
FROM         
                      GeneratedCalls  inner JOIN
                      RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID LEFT OUTER JOIN
                      Sources ON RecievedCalls.SourceID = Sources.ID LEFT OUTER JOIN
                      SourceKinds ON SourceKinds.ID = Sources.SourceKindID
WHERE  (@ClientID=0 or dbo.RecievedCalls.ClientID=@ClientID) and  ( @MobileOperatorID =0 or  GeneratedCalls.MobileOperatorID=@MobileOperatorID) and         (RecievedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate)
GROUP BY GeneratedCalls.OriginationNetwork, GeneratedCalls.MobileOperatorID, SourceKinds.Name
ORDER BY GeneratedCalls.OriginationNetwork, SourceKind







END