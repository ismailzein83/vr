
CREATE VIEW [ICX_SMS].[vw_ICXSMSInvalidAndRaw]
AS
SELECT invalidSMS.Id, invalidSMS.GatewayId as Gateway, invalidSMS.SentDateTime, invalidSMS.OperatorID, 
	   invalidSMS.OriginationMCC, invalidSMS.OriginationMNC, invalidSMS.OriginationMobileNetwork,invalidSMS.OriginationMobileCountry,
	   invalidSMS.DestinationMCC,invalidSMS.DestinationMNC, invalidSMS.DestinationMobileNetwork, invalidSMS.DestinationMobileCountry,
	   invalidSMS.Sender, invalidSMS.Receiver, invalidSMS.BillingType, invalidSMS.TrafficDirection, sms.Trunk
FROM   [Retail_Dev_ICX_SMS].[ICX_SMS].[BillingSMS_Invalid] AS invalidSMS WITH (NOLOCK) 
INNER JOIN  [Retail_Dev_ICX_SMS].[ICX_SMS].[SMS] AS sms WITH (NOLOCK) ON invalidSMS.Id = sms.Id