

CREATE PROCEDURE [TOneWhS_BE].[sp_ZoneRoutingProduct_Update]
	@OwnerId int,
	@OwnerType int,
	@ZoneId bigint,
	@BED Datetime,
	@RoutingProductId bigint,
	@ReservedId bigint,
	@ZoneRoutingProductToClose [TOneWhS_BE].[RoutingProductToClose] READONLY

AS
BEGIN
BEGIN TRY
Begin Transaction routingProductTransaction

	Update zrp set zrp.EED = zrptc.RoutingProductEED
	from [TOneWhS_BE].SaleEntityRoutingProduct zrp
	join @ZoneRoutingProductToClose zrptc on zrp.ID=zrptc.RoutingProductId

	INSERT INTO [TOneWhS_BE].[SaleEntityRoutingProduct]
           ([ID]
		   ,[OwnerType]
           ,[OwnerID]
           ,[ZoneID]
           ,[RoutingProductID]
           ,[BED])
		   VALUES
           (@ReservedId
		   ,@OwnerType
           ,@OwnerId
           ,@ZoneId
           ,@RoutingProductId
           ,@BED)
	
	COMMIT Transaction routingProductTransaction
END TRY
BEGIN CATCH
	ROLLBACK Transaction routingProductTransaction
END CATCH
END