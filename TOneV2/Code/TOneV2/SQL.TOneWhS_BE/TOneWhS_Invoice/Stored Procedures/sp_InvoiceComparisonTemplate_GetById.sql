-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [TOneWhS_Invoice].[sp_InvoiceComparisonTemplate_GetById]
	@InvoiceTypeId uniqueidentifier,
	@PartnerId nvarchar(255)
AS
BEGIN
	SELECT	InvoiceTypeId,PartnerId,Details
	FROM	[TOneWhS_Invoice].[InvoiceComparisonTemplate] with(nolock)
	WHERE	InvoiceTypeId=@InvoiceTypeId AND PartnerId=@PartnerId
END