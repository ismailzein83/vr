CREATE VIEW [TOneWhS_SMS].[vw_SMSInvalidAndRaw]
AS
SELECT invalidSMS.SMSId, invalidSMS.SwitchId as Switch, invalidSMS.SentDateTime, invalidSMS.CustomerID, invalidSMS.SupplierID, invalidSMS.DestinationMCC,
	   invalidSMS.DestinationMNC, invalidSMS.DestinationMN_Id, invalidSMS.Receiver, invalidSMS.OrigReceiverOut, invalidSMS.CostRateId, 
	   invalidSMS.SaleRateId, invalidSMS.SaleFinancialAccountId, invalidSMS.CostFinancialAccountId, 
	   sms.IN_CARRIER as InCarrier, sms.OUT_CARRIER as OutCarrier
FROM   TOneWhS_SMS.BillingSMS_Invalid AS invalidSMS WITH (NOLOCK) 
INNER JOIN  TOneWhS_SMS.SMS AS sms WITH (NOLOCK) ON invalidSMS.SMSId = sms.Id