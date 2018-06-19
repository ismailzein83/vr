-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>

-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].[sp_ReceivedSupplierPricelist_SetAsStarted]
	@ReceivedPricelistId int,
	@Status smallint,
	@ProcessInstanceId bigint,
	@StartProcessingDate datetime
AS
BEGIN
		UPDATE [TOneWhS_SPL].ReceivedSupplierPricelist 
		SET [Status] = @Status,
			[StartProcessingDate] = @StartProcessingDate,
			[ProcessInstanceId] = @ProcessInstanceId
		WHERE ID = @ReceivedPricelistId
END