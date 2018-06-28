-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>

-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].sp_ReceivedSupplierPricelist_SetSentToSupplierStatus
	@ReceivedPricelistId int,
	@Status bit
AS
BEGIN
		UPDATE [TOneWhS_SPL].ReceivedSupplierPricelist 
		SET [SentToSupplier] = @Status
		WHERE ID = @ReceivedPricelistId
END