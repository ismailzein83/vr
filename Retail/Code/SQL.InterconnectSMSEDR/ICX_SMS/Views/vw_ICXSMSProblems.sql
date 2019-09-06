
CREATE VIEW [ICX_SMS].[vw_ICXSMSProblems]
AS
SELECT ID, GatewayID, SentDateTime, OperatorID, OriginationMCC,  OriginationMNC, OriginationMobileNetwork, OriginationMobileCountry,
	   DestinationMCC, DestinationMNC, DestinationMobileNetwork, DestinationMobileCountry, Sender , Receiver, Amount, BillingType, TrafficDirection
FROM  [ICX_SMS].[BillingSMS_Invalid] WITH (NOLOCK)