-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZoneService_CloseOverlappedDefaultService]
	-- Add the parameters for the stored procedure here
	@ExistingId bigint,
	@SupplierZoneServiceId bigint,
	@SupplierId int,
	@ReceivedServicesFlag nvarchar(max),
	@EffectiveServiceFlag nvarchar(max),
	@When Datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [TOneWhS_BE].[SupplierZoneService]
	SET 
		[TOneWhS_BE].[SupplierZoneService].EED=@When
		where ID=@ExistingId
		
	Insert into TOneWhS_BE.SupplierZoneService(ID,SupplierID,ReceivedServicesFlag,EffectiveServiceFlag, BED)
	Values(@SupplierZoneServiceId, @SupplierId, @ReceivedServicesFlag, @EffectiveServiceFlag, @When)	 
END