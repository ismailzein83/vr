-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>

-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].[sp_ReceivedSupplierPricelist_SetAsCompleted]
	@receivedPricelistId int,
	@Status smallint,
	@PricelistId int
AS
BEGIN
		UPDATE [TOneWhS_SPL].ReceivedSupplierPricelist 
		SET [Status] = @Status, PricelistID = @PricelistId
		WHERE ID = @receivedPricelistId
END