















CREATE PROCEDURE [dbo].[prVwCarrier]
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


		SELECT   distinct  (CASE WHEN Carriers.Name IS NULL THEN 'Route ' + RIGHT('000' + CONVERT(VARCHAR(4),   case when   GeneratedCalls.Type ='GSM' then 5 else 201 end   ) , 4) WHEN Carriers.Name = '' THEN 'Route ' + RIGHT('000' + CONVERT(VARCHAR(4),   case when   GeneratedCalls.Type ='GSM' then 5 else 201 end   ) , 4) ELSE 'Route ' + RIGHT('000' + CONVERT(VARCHAR(4), Carriers.ID), 4) END) AS Carrier, 
							   COUNT(*) AS count, GeneratedCalls.Type AS SourceKind
		FROM         Carriers INNER JOIN
							  GeneratedCalls ON Carriers.Name = GeneratedCalls.Carrier inner JOIN
							  RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID 
		WHERE          (RecievedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate) and (@ClientID=0 or dbo.RecievedCalls.ClientID=@ClientID) and     ( @MobileOperatorID =0 or  GeneratedCalls.MobileOperatorID=@MobileOperatorID) 
		GROUP BY Carriers.Name, GeneratedCalls.Type, Carriers.ID
		ORDER BY Carrier, SourceKind


	End
	else
	Begin


		SELECT   distinct  (CASE WHEN GeneratedCalls.Carrier IS NULL THEN 'Unknown Route' WHEN GeneratedCalls.Carrier = '' THEN 'Unknown Route' ELSE GeneratedCalls.Carrier End )AS Carrier,  
							   COUNT(*) AS count, GeneratedCalls.Type AS SourceKind
		FROM         GeneratedCalls inner join
							  RecievedCalls ON GeneratedCalls.ID = RecievedCalls.GeneratedCallID 
		WHERE     (RecievedCalls.AttemptDateTime BETWEEN @StartDate AND @EndDate) and (@ClientID=0 or ClientID=@ClientID) and (@MobileOperatorID=0 or dbo.GeneratedCalls.MobileOperatorID=@MobileOperatorID ) 
		GROUP BY GeneratedCalls.Carrier,  GeneratedCalls.Type
		ORDER BY Carrier, SourceKind


	End







END