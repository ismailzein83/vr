


CREATE VIEW [ICX_CDR].[vw_ICXCDRProblems]
AS
SELECT CDRId, SwitchId, AttemptDateTime, DurationInSeconds, CGPN, CDPN, OperatorID, OriginationZoneId, DestinationZoneId,Rate, BillingType, TrafficDirection
FROM  [ICX_CDR].[BillingCDR_Invalid] WITH (NOLOCK)