CREATE procedure [dbo].[GetSettlementInvoices]
    (@carrierId varchar(10), @BeginDate datetime , @EndDate datetime)
AS

set nocount on

SELECT [Id]
      ,[IssueDate]
      ,[DueDate]
      ,[FromDate]
      ,[ToDate]
  FROM [SettlementInvoice]
  where Id IN(
	SELECT SettlementInvoiceId
	FROM [SettlementInvoiceCurrency]
	where  (SupplierID = @carrierId or CustomerId = @carrierId)  
  )
  and ( FromDate >= @BeginDate and ToDate <= @EndDate )