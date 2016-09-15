-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetFiltered]
	@InvoiceTypeId uniqueidentifier,
	@PartnerID nvarchar(50),
	@FromDate datetime,
	@ToDate datetime
AS
BEGIN
	SELECT	ID,InvoiceTypeID,PartnerID,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details
	FROM	VR_Invoice.Invoice with(nolock)
	where	(InvoiceTypeId = @InvoiceTypeId) AND (@PartnerID is null OR PartnerID = @PartnerID) 
			AND FromDate >= @FromDate AND (@ToDate is null OR ToDate <= @ToDate)
END