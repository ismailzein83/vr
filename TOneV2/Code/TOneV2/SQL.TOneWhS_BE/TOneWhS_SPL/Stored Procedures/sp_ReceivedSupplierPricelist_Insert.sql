-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>

-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].[sp_ReceivedSupplierPricelist_Insert]
	@SupplierId int,
	@FileId  bigint,
	@ReceivedDate  datetime,
	@PricelistType smallint,
	@Status smallint,
	@PricelistId int,
	@ProcessInstanceId bigint,
	@serializedErrors nvarchar(max),
	@ID INT OUT
AS
BEGIN
		INSERT INTO [TOneWhS_SPL].ReceivedSupplierPricelist ( SupplierID, FileID, ReceivedDate, PricelistType, [Status], PricelistID, ProcessInstanceId,ErrorDetails)
		VALUES ( @SupplierId,  @FileId,@ReceivedDate,@PricelistType,@Status,@PricelistId,@ProcessInstanceId,@serializedErrors)
		SET @ID = SCOPE_IDENTITY()
END