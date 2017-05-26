-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZoneService_Update]
	@Id bigint,
	@SupplierId int,
	@ZoneId bigint,
	@ReceivedServicesFlag nvarchar(max),
	@EffectiveServiceFlag nvarchar(max),
	@BED Datetime,
	@SupplierZoneServicesToClose [TOneWhS_BE].[SupplierZoneServiceClose] READONLY

AS
BEGIN
BEGIN TRY
Begin Transaction supplierZoneServiceTransaction

	Update szs set szs.EED = sz.SupplierZoneSeviceEEDToClose
	from [TOneWhS_BE].[SupplierZoneService] szs
	join @SupplierZoneServicesToClose sz on sz.SupplierZoneSeviceIdToClose=szs.ID

	Insert into TOneWhS_BE.SupplierZoneService(ID,ZoneID,SupplierID,ReceivedServicesFlag,EffectiveServiceFlag, BED)
	Values(@Id,@zoneId, @SupplierId, @ReceivedServicesFlag, @EffectiveServiceFlag, @BED)
	
	COMMIT Transaction supplierZoneServiceTransaction
END TRY
BEGIN CATCH
	ROLLBACK Transaction supplierZoneServiceTransaction
END CATCH
END