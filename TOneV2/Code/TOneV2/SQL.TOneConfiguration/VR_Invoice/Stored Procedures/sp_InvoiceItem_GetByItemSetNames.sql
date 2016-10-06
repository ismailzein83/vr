CREATE PROCEDURE [VR_Invoice].[sp_InvoiceItem_GetByItemSetNames]
	@InvoiceId bigint,
	@ItemSetNames varchar(max)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @ItemSetNamesTable TABLE (ItemSetName nvarchar(255))
	INSERT INTO @ItemSetNamesTable (ItemSetName)
	SELECT CONVERT(nvarchar(255), ParsedString) FROM [VR_Invoice].[ParseStringList](@ItemSetNames)
	
 SELECT [ID],InvoiceID,ItemSetName,Name,Details
 FROM	[VR_Invoice].InvoiceItem with(nolock)
 WHERE	InvoiceID =@InvoiceId  AND  ItemSetName in (SELECT ItemSetName FROM @ItemSetNamesTable)
End