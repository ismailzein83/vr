
CREATE procedure [dbo].[GetSettlementInvoiceCurrency]
    (@SettlementInvoiceId int)
AS

set nocount on

SELECT [Id]
      ,[SettlementInvoiceId]
      ,[BeginDate]
      ,[EndDate]
      ,[IssueDate]
      ,[DueDate]
      ,[SupplierId]
      ,[CustomerId]
      ,[Amount]
      ,[CurrencyId]
      ,[IsLocked]
      ,[SourceFileName]
FROM [SettlementInvoiceCurrency]
where  SettlementInvoiceId = @SettlementInvoiceId