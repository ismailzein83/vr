-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_BE].[sp_SupplierZone_Update]
	-- Add the parameters for the stored procedure here
	@SupplierID int,
	@When datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [TOneWhS_BE].[SupplierZone]
	SET 
		[TOneWhS_BE].[SupplierZone].EED=@When
	FROM [TOneWhS_BE].[SupplierZone] sz where sz.SupplierID=@SupplierID 
	 and (sz.EED is null or sz.EED > @when)
END