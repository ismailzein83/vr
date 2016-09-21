-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZonesServices_GetByDate] 
	-- Add the parameters for the stored procedure here
	@SupplierId INT,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  szs.[ID],szs.[PriceListID],szs.[ZoneID],szs.[ReceivedServicesFlag],szs.[EffectiveServiceFlag],szs.[BED],szs.[EED]
	FROM	[TOneWhS_BE].SupplierZoneService szs WITH(NOLOCK) 
			LEFT JOIN [TOneWhS_BE].SupplierZone sz WITH(NOLOCK) ON szs.ZoneID=sz.ID 
	Where	(szs.EED is null or szs.EED > @when)
			and sz.SupplierID=@SupplierId
	  
END