
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZone_UpdateName]
	@ID int,
	@Name nvarchar(max),
	@SellingNumberPlanID int
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.SaleZone WHERE ID != @Id AND Name = @Name AND SellingNumberPlanID=@SellingNumberPlanID)
	BEGIN
	BEGIN TRAN

	Update TOneWhS_BE.SaleZone
	Set Name = @Name
	Where ID = @ID

	Update [TOneV2_Dev].[TOneWhS_BE].[SalePricelistRateChange]
	Set ZoneName = @Name
	Where ZoneID = @ID

	Update [TOneV2_Dev].[TOneWhS_BE].[SalePricelistCodeChange]
	Set ZoneName = @Name
	Where ZoneID = @ID

	COMMIT TRAN
	END



	
END