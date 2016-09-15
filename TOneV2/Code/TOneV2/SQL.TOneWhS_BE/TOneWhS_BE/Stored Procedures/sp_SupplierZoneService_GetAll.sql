CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZoneService_GetAll]
	
AS
BEGIN

	SET NOCOUNT ON;

SELECT  supzs.ID,
        supzs.ZoneID,
        supzs.ReceivedServicesFlag,
        supzs.EffectiveServiceFlag,
        supzs.BED,
        supzs.EED

FROM	[TOneWhS_BE].SupplierZoneService supzs WITH(NOLOCK) 

END