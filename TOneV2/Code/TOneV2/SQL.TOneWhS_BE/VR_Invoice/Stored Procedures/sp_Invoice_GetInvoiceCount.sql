-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetInvoiceCount]
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50),
		@FromDate datetime,
		@ToDate datetime 
AS
BEGIN
	SELECT	Count(*) as [Counter]
	FROM	VR_Invoice.Invoice with(nolock)
	where	InvoiceTypeID = @InvoiceTypeId AND  (@PartnerId is null  or PartnerId = @PartnerId) AND (@FromDate is null or IssueDate >= @FromDate ) AND (@ToDate is null or IssueDate <= @ToDate )
END