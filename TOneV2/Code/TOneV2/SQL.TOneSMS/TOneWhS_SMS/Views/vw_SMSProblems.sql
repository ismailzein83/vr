




CREATE VIEW [TOneWhS_SMS].[vw_SMSProblems]
AS
SELECT SMSId, SwitchId, SentDateTime, CustomerID, SupplierID, DestinationMCC, DestinationMNC, DestinationMN_Id, Receiver, OrigReceiverOut, CostRateId, 
	   SaleRateId, SaleFinancialAccountId, CostFinancialAccountId
FROM  TOneWhS_SMS.BillingSMS_Invalid WITH (NOLOCK)

UNION ALL

SELECT SMSId, SwitchId, SentDateTime, CustomerID, SupplierID, DestinationMCC, DestinationMNC, DestinationMN_Id, Receiver, OrigReceiverOut, CostRateID, 
	   SaleRateID, SaleFinancialAccountId, CostFinancialAccountId
FROM  TOneWhS_SMS.BillingSMS_PartialPriced WITH (NOLOCK)