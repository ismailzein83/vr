-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_Update]
	-- Add the parameters for the stored procedure here
	@SupplierZoneIDs  varchar(max),
	@When datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		DECLARE @SupplierZoneIdsTable TABLE (SupplierZoneId bigint)
		INSERT INTO @SupplierZoneIdsTable (SupplierZoneId)
		select Convert(bigint, ParsedString) from [TOneWhS_BE].[ParseStringList](@SupplierZoneIDs)
	UPDATE [TOneWhS_BE].[SupplierRate]
	SET 
		[TOneWhS_BE].[SupplierRate].EED=@When
	FROM [TOneWhS_BE].[SupplierRate] sz where (sz.ZoneID IN (SELECT SupplierZoneId FROM @SupplierZoneIdsTable))
	 and (sz.EED is null or sz.EED > @when)
END