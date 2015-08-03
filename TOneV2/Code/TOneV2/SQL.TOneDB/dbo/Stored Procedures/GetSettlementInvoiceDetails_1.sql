
CREATE procedure [dbo].[GetSettlementInvoiceDetails]
(@Id int)
AS

set nocount on

SELECT   dbo.SettlementInvoiceDetails.SettlementInvoiceId,
		 dbo.SettlementInvoiceDetails.InvoiceId, 
		 dbo.SettlementInvoiceDetails.BeginDate, 
		 dbo.SettlementInvoiceDetails.EndDate, 
		 dbo.SettlementInvoiceDetails.IssueDate, 
		 dbo.SettlementInvoiceDetails.DueDate, 
		 dbo.SettlementInvoiceDetails.SupplierID, 
		 dbo.SettlementInvoiceDetails.CustomerID, 
		 dbo.SettlementInvoiceDetails.SerialNumber, 
		 dbo.SettlementInvoiceDetails.Duration, 
		 dbo.SettlementInvoiceDetails.Amount, 
		 dbo.SettlementInvoiceDetails.CurrencyID, 
		 dbo.SettlementInvoiceDetails.IsLocked, 
		 dbo.SettlementInvoiceDetails.IsPaid, 
		 dbo.SettlementInvoiceDetails.PaidDate, 
         dbo.SettlementInvoiceDetails.InvoicePrintedNote, 
         dbo.SettlementInvoiceDetails.UserID, 
         dbo.SettlementInvoiceDetails.InvoiceNotes, 
         dbo.SettlementInvoiceDetails.IsSent, 
         dbo.SettlementInvoiceDetails.VatValue, 
         dbo.SettlementInvoiceDetails.CustomerMask, 
         dbo.SettlementInvoiceDetails.IsAutomatic, 
		 dbo.SettlementInvoiceDetails.PaidAmount, 
		 dbo.SettlementInvoiceDetails.NumberOfCalls, 
		 dbo.SettlementInvoiceActualAmount.BeginDate AS  ActualBeginDate,
		 dbo.SettlementInvoiceActualAmount.InvoiceId AS  ActualInvoiceId,
		 dbo.SettlementInvoiceActualAmount.EndDate AS  ActualEndDate,
		 dbo.SettlementInvoiceActualAmount.IssueDate AS  ActualIssueDate,
		 dbo.SettlementInvoiceActualAmount.DueDate AS ActualDueDate,
		 dbo.SettlementInvoiceActualAmount.CarrierAccountId , 
		 dbo.SettlementInvoiceActualAmount.Duration AS ActualDuration, 
		 dbo.SettlementInvoiceActualAmount.Calls AS ActualCalls,  
		 dbo.SettlementInvoiceActualAmount.Amount AS ActualAmount, 
		 dbo.SettlementInvoiceActualAmount.CurrencyId AS ActualCurrency, 
		 dbo.SettlementInvoiceActualAmount.IsLocked AS ActualIsLocked, 
		 dbo.SettlementInvoiceActualAmount.ID 
		 
FROM     dbo.SettlementInvoiceActualAmount RIGHT OUTER JOIN
         dbo.SettlementInvoiceDetails ON dbo.SettlementInvoiceActualAmount.SettlementInvoiceDetailId = dbo.SettlementInvoiceDetails.ID
where    dbo.SettlementInvoiceDetails.[SettlementInvoiceId] = @Id