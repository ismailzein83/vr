-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_SupplierCode_Update]
	-- Add the parameters for the stored procedure here
	@SupplierCodeIDs  varchar(max),
	@When datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		DECLARE @SupplierCodeIdsTable TABLE (SupplierCodeId bigint)
		INSERT INTO @SupplierCodeIdsTable (SupplierCodeId)
		select Convert(bigint, ParsedString) from [TOneWhS_BE].[ParseStringList](@SupplierCodeIDs)
	UPDATE [TOneWhS_BE].[SupplierCode]
	SET 
		[TOneWhS_BE].[SupplierCode].EED=@When
	FROM [TOneWhS_BE].[SupplierCode] sc where (sc.ZoneID IN (SELECT SupplierCodeId FROM @SupplierCodeIdsTable))
	 and (sc.EED is null or sc.EED > @when)
END