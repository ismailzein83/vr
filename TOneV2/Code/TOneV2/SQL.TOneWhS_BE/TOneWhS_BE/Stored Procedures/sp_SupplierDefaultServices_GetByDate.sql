-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierDefaultServices_GetByDate] 
	-- Add the parameters for the stored procedure here
	@SupplierId INT,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  [ID],[PriceListID],[ZoneID], [SupplierID],[ReceivedServicesFlag],[EffectiveServiceFlag],[BED],[EED]
	FROM	[TOneWhS_BE].SupplierZoneService WITH(NOLOCK) 	
	Where	(EED is null or EED > @when)
			and SupplierID=@SupplierId and ZoneID is null
	ORDER BY BED ASC
	  
END