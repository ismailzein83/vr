-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZone_Update]
	-- Add the parameters for the stored procedure here
	@SaleZones [TOneWhS_BE].[SaleData] READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [TOneWhS_BE].[SaleZone]
	SET 
		[TOneWhS_BE].[SaleZone].EED=sz.EED
	FROM [TOneWhS_BE].[SaleZone]  inner join @SaleZones as sz ON  [TOneWhS_BE].[SaleZone].ID = sz.ID
END