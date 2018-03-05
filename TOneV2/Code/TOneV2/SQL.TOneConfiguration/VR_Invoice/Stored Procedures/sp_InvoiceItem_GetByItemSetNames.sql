CREATE PROCEDURE [VR_Invoice].[sp_InvoiceItem_GetByItemSetNames]
	@InvoiceIds nvarchar(MAX),
	@ItemSetNames varchar(max)
AS
BEGIN
SET NOCOUNT ON;
		DECLARE @ItemSetNamesTable TABLE (ItemSetName nvarchar(255))
		INSERT INTO @ItemSetNamesTable (ItemSetName)
		SELECT CONVERT(nvarchar(255), ParsedString) FROM [VR_Invoice].[ParseStringList](@ItemSetNames)
	
		 DECLARE @InvoiceIdsTable TABLE (InvoiceId BIGINT)
		 INSERT INTO @InvoiceIdsTable (InvoiceId)
		 select ParsedString from [VR_Invoice].[ParseStringList](@InvoiceIds)


	    SELECT [ID],InvoiceID,invItem.ItemSetName,Name,Details
	    FROM	[VR_Invoice].InvoiceItem invItem with(nolock)
	    JOIN @ItemSetNamesTable setName ON invItem.ItemSetName like setName.ItemSetName
	    WHERE	InvoiceID  IN (select InvoiceId from @InvoiceIdsTable)
End