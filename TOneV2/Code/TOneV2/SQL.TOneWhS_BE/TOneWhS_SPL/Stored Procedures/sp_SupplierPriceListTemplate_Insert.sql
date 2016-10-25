-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>

-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].[sp_SupplierPriceListTemplate_Insert]
	@SupplierID int,
	@ConfigDetails  nvarchar(MAX),
	@Draft  nvarchar(MAX),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM [TOneWhS_SPL].SupplierPriceListTemplate WHERE  SupplierID = @SupplierID )
	BEGIN
		INSERT INTO [TOneWhS_SPL].SupplierPriceListTemplate ( SupplierID,  ConfigDetails,Draft)
		VALUES ( @SupplierID,  @ConfigDetails,@Draft)
		SET @ID = SCOPE_IDENTITY()
	END
END