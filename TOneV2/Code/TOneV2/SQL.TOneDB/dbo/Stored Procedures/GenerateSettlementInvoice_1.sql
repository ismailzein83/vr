CREATE procedure [dbo].[GenerateSettlementInvoice]
    (@carrierId varchar(10), @BeginDate datetime , @EndDate datetime)
AS

set nocount on

;with TEMP as (
	SELECT 'Supplier' as InvoiceType, InvoiceID,CurrencyID ,SUM(Duration) as Duration ,SUM(NumberOfCalls)  as NumberOfCalls ,SUM(Amount) as Amount 
	FROM [dbo].[Billing_Invoice_Details] with(nolock)
	where InvoiceID in ( SELECT InvoiceID
						 FROM [dbo].[Billing_Invoice] with(nolock)
						 where  SupplierID = @carrierId and BeginDate >= @BeginDate and EndDate <= @EndDate )
	group by CurrencyID , InvoiceID
	Union
	SELECT 'Customer' as InvoiceType,InvoiceID,CurrencyID  ,SUM(Duration) as Duration ,SUM(NumberOfCalls)  as NumberOfCalls , SUM(Amount) as Amount 
	FROM [dbo].[Billing_Invoice_Details] with(nolock)
	where InvoiceID in ( SELECT InvoiceID
						 FROM [dbo].[Billing_Invoice] with(nolock)
						 where  CustomerID = @carrierId  and BeginDate >= @BeginDate and EndDate <= @EndDate )
	group by CurrencyID , InvoiceID 
)

SELECT TEMP.InvoiceID as  InvoiceID ,  
       TEMP.InvoiceType  as InvoiceType  ,
       TEMP.CurrencyID  as CurrencyID  ,
       TEMP.Amount as Amount ,
       TEMP.Duration  as Duration ,
       TEMP.NumberOfCalls as NumberOfCalls ,
       BInvoice.IssueDate as IssueDate ,
       BInvoice.DueDate as DueDate ,
       BInvoice.SupplierID as SupplierID ,
       BInvoice.CustomerID as CustomerID ,
       BInvoice.SerialNumber as SerialNumber ,
       BInvoice.IsLocked as IsLocked ,
       BInvoice.IsPaid as IsPaid ,
       BInvoice.PaidDate as PaidDate ,
       BInvoice.UserID as UserID ,
       BInvoice.InvoiceAttachement as InvoiceAttachement ,
       BInvoice.FileName as FileName ,
       BInvoice.InvoicePrintedNote as InvoicePrintedNote ,
       BInvoice.InvoiceNotes as InvoiceNotes ,
       BInvoice.PaidAmount as PaidAmount ,
       BInvoice.VatValue as VatValue ,
       BInvoice.SourceFileName as SourceFileName ,
       BInvoice.IsSent as IsSent ,
       BInvoice.CustomerMask as CustomerMask ,
       BInvoice.IsAutomatic as IsAutomatic,
       BInvoice.PaidAmount as PaidAmount ,
       SettlementInvoiceActualAmount.ID    as  AID ,
	   SettlementInvoiceActualAmount.InvoiceId    as  AInvoiceId ,
		SettlementInvoiceActualAmount.BeginDate    as  ABeginDate ,
		SettlementInvoiceActualAmount.EndDate    as  AEndDate ,
		SettlementInvoiceActualAmount.IssueDate    as  AIssueDate ,
		SettlementInvoiceActualAmount.DueDate    as  ADueDate ,
		SettlementInvoiceActualAmount.CarrierAccountId    as  ACarrierAccountId ,
		SettlementInvoiceActualAmount.Duration    as  ADuration ,
		SettlementInvoiceActualAmount.Calls    as  ACalls ,
		SettlementInvoiceActualAmount.Amount    as  AAmount ,
		SettlementInvoiceActualAmount.CurrencyId    as  ACurrencyId ,
		SettlementInvoiceActualAmount.IsLocked    as  AIsLocked ,
		SettlementInvoiceActualAmount.SourceFileName    as  ASourceFileName ,
		SettlementInvoiceActualAmount.InvoiceAttachement    as  AInvoiceAttachement 
       
FROM  dbo.SettlementInvoiceActualAmount  RIGHT OUTER JOIN
      TEMP   ON  dbo.SettlementInvoiceActualAmount.InvoiceId = TEMP.InvoiceID and 
				 dbo.SettlementInvoiceActualAmount.CurrencyId = TEMP.CurrencyID 
      inner join Billing_Invoice as BInvoice with(nolock) on (BInvoice.InvoiceID = TEMP.InvoiceID)