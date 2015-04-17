





CREATE PROCEDURE [dbo].[prVwOrigination]
(
 @MobileOperatorID INT = NULL
,@StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
,@ClientID INT = NULL
,@IsAdmin bit = 0
)
AS
BEGIN    




if (@IsAdmin=0)
	Begin


		select  s.OriginationNetwork,Sum(s.Count)AS count from 

		(SELECT   distinct  (CASE WHEN GeneratedCalls.OriginationNetwork IS NULL 
							  THEN 'INTERNATIONAL' WHEN GeneratedCalls.OriginationNetwork = '' THEN 'INTERNATIONAL' ELSE GeneratedCalls.OriginationNetwork END) 
							  AS OriginationNetwork,   COUNT(*) AS count
		FROM         
							  GeneratedCalls  inner JOIN
							  RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID 
		WHERE (RecievedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate) and (@ClientID=0 or   ClientID=@ClientID )    and   (@MobileOperatorID=0 or dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID )            
		GROUP BY GeneratedCalls.OriginationNetwork) as s
		
		Group by s.OriginationNetwork
		
		order by s.OriginationNetwork


	End
	else
	Begin


		select  s.OriginationNetwork,Sum(s.Count)AS count from 

(SELECT   distinct  (CASE WHEN GeneratedCalls.OriginationNetwork IS NULL 
                      THEN 'INTERNATIONAL' WHEN GeneratedCalls.OriginationNetwork = '' THEN 'INTERNATIONAL' ELSE GeneratedCalls.OriginationNetwork END) 
                      AS OriginationNetwork,  COUNT(*) AS Count
		FROM         
							  GeneratedCalls  inner JOIN
							  RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID 
		WHERE (RecievedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate) and (@ClientID=0 or   ClientID=@ClientID )    and   (@MobileOperatorID=0 or dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID )            
		
		GROUP BY GeneratedCalls.OriginationNetwork) as s
		
		Group by s.OriginationNetwork
		
		order by s.OriginationNetwork


	End














END