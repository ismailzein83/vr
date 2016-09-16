-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_BE].[sp_SaleEntityService_GetEffectiveZoneServicesByOwner]
-- Add the parameters for the stored procedure here
@EffectiveTime DATETIME = NULL,
@IsFuture bit,
@SaleEntityZoneServiceOwner [TOneWhS_BE].[RoutingOwnerInfo] READONLY
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;


select	ses.[ID], ses.[PriceListID], ses.[ZoneID], ses.[Services], ses.[BED], ses.[EED]
from	TOneWhS_BE.SaleEntityService ses 
		JOIN [TOneWhS_BE].SalePriceList spl with(nolock) ON spl.ID = ses.PriceListID
		JOIN @SaleEntityZoneServiceOwner sezso on sezso.OwnerId = spl.OwnerId and sezso.OwnerTpe = spl.OwnerType
Where	((@IsFuture = 0 AND ses.BED <= @EffectiveTime AND (ses.EED > @EffectiveTime OR ses.EED IS NULL))
		OR (@IsFuture = 1 AND (ses.BED > GETDATE() OR ses.EED IS NULL)))
		group by ses.[ID], ses.[PriceListID], ses.[ZoneID], ses.[Services], ses.[BED], ses.[EED]
END