-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZoneService_Insert]
    @Id bigint,
	@SupplierId int,
	@ReceivedServicesFlag nvarchar(max),
	@EffectiveServiceFlag nvarchar(max),
	@BED Datetime,
	@EED Datetime
AS
BEGIN
	Insert into TOneWhS_BE.SupplierZoneService(ID,SupplierID,ReceivedServicesFlag,EffectiveServiceFlag, BED, EED)
	Values(@Id, @SupplierId, @ReceivedServicesFlag, @EffectiveServiceFlag, @BED, @EED)
END