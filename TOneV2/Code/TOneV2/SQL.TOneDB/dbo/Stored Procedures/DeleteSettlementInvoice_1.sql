CREATE procedure [dbo].[DeleteSettlementInvoice]
    (@id int)
AS

set nocount on

BEGIN TRAN T1;

DELETE FROM [dbo].[SettlementInvoice] WHERE SettlementInvoice.Id  = @id

DELETE FROM [SettlementInvoiceActualAmount] WHERE  SettlementInvoiceActualAmount.SettlementInvoiceDetailId In (
			SELECT [ID]
			FROM [dbo].SettlementInvoiceDetails 
			where SettlementInvoiceId = @id
			)
DELETE FROM [SettlementInvoiceDetails] WHERE SettlementInvoiceId  = @id

DELETE FROM [SettlementInvoiceCurrency]
	WHERE SettlementInvoiceId  = @id
	
COMMIT TRAN T1;