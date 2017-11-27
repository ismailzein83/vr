-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Invoice].[sp_InvoiceComparisonTemplate_TryAddOrUpdate]
@InvoiceTypeId  uniqueidentifier,
@PartnerId  nvarchar(255),
@Details nvarchar(MAX)

AS
BEGIN

	IF EXISTS(SELECT 1 from  [TOneWhS_Invoice].[InvoiceComparisonTemplate] WHERE InvoiceTypeId = @InvoiceTypeId AND PartnerId=@PartnerId)
	BEGIN
		UPDATE [TOneWhS_Invoice].[InvoiceComparisonTemplate]
		SET		Details = @Details
		WHERE InvoiceTypeId = @InvoiceTypeId AND PartnerId=@PartnerId
	END
	ELSE
	BEGIN
		INSERT INTO [TOneWhS_Invoice].[InvoiceComparisonTemplate](InvoiceTypeId,PartnerId,Details)
		VALUES ( @InvoiceTypeId, @PartnerId,@Details)	
	END
END