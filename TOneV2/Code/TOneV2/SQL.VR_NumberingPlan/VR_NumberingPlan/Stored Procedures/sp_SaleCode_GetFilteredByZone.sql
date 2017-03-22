-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleCode_GetFilteredByZone]
	@ZoneId bigint,
	@EffectiveOn datetime
AS
BEGIN
	select Id, Code, ZoneId, CodeGroupId, BED, EED, SourceId
	from TOneWhS_BE.SaleCode
	where ZoneId = @ZoneId and (EED is null or (BED != EED and EED > @EffectiveOn))
END