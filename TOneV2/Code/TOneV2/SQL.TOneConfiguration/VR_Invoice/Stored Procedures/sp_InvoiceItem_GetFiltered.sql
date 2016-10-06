-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceItem_GetFiltered]
	@InvoiceId bigint,
	@ItemSetName  nvarchar(255)
AS
BEGIN
	SELECT	ID,InvoiceId,ItemSetName,Name,Details
	FROM	VR_Invoice.InvoiceItem with(nolock)
	where	(@InvoiceId is null OR InvoiceId = @InvoiceId) AND ItemSetName = @ItemSetName
END