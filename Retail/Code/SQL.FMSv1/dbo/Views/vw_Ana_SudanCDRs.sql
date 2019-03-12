
CREATE VIEW [dbo].[vw_Ana_SudanCDRs]
AS
SELECT        r.CLI, r.b_number, r.AttemptDateTime, CASE WHEN r.[OriginationNetwork] IS NULL OR
                r.[OriginationNetwork] = '' THEN 'INTERNATIONAL' ELSE r.[OriginationNetwork] END AS OriginationNetwork, 
				CASE WHEN r.[OriginationNetwork] IS NULL OR r.[OriginationNetwork] = '' THEN 'SIP' ELSE 'GSM' END AS Type, 
				CASE WHEN ((r.CLI LIKE '011%' OR r.CLI LIKE '012%')) AND (LEN(r.CLI) = 10) THEN 1 
					 WHEN (r.CLI LIKE '0%') AND (r.CLI NOT LIKE '00%') AND (LEN(r.CLI) = 10) THEN 2 
					 ELSE 3 END AS CaseAndNetworkType, 
				CASE WHEN (r.CLI LIKE '0%') AND (r.CLI NOT LIKE '00%') AND (LEN(r.CLI) = 10) THEN 1 
					 ELSE 2 END AS CaseType
FROM            dbo.RecievedCalls AS r 
		INNER JOIN dbo.GeneratedCalls AS g ON r.GeneratedCallID = g.ID
WHERE        (r.b_number LIKE '249%') --AND (g.a_number <> 'Unknown') 