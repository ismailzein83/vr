-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>

-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].[sp_ReceivedSupplierPricelist_UpdateStatus]
	@ReceivedPricelistId int,
	@Status smallint
AS
BEGIN
		UPDATE [TOneWhS_SPL].ReceivedSupplierPricelist 
		SET [Status] = @Status
		WHERE ID = @ReceivedPricelistId and [Status] < 50
END