-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].sp_Invoice_CheckOverlaping
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50),
		@FromDate Datetime,
		@ToDate Datetime
AS
BEGIN
	SELECT	Count(*) as CountNb
	FROM	VR_Invoice.Invoice with(nolock)
	where	PartnerId = @PartnerId AND InvoiceTypeId = @InvoiceTypeId AND
	(ToDate >= @FromDate AND  @ToDate>=FromDate)
END