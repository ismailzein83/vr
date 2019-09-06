

CREATE VIEW [ICX_CDR].[vw_ICXCDRInvalidAndRaw]
AS
SELECT invalidCDR.CDRId, invalidCDR.SwitchId, invalidCDR.AttemptDateTime, invalidCDR.DurationInSeconds, invalidCDR.CGPN, invalidCDR.CDPN, invalidCDR.OperatorID, 
	   invalidCDR.OriginationZoneID, invalidCDR.DestinationZoneID, invalidCDR.BillingType, invalidCDR.TrafficDirection, cdr.InTrunk, cdr.OutTrunk
FROM   ICX_CDR.BillingCDR_Invalid AS invalidCDR WITH (NOLOCK) 
INNER JOIN  ICX_CDR.CDR AS cdr WITH (NOLOCK) ON invalidCDR.CDRId = cdr.Id