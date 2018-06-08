-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_ZoneCodeGroup_GetCost]
	@EffectiveOn DateTime,
	@IsFuture BIT
AS
BEGIN
	SELECT ZoneID, sc.Code as CodeGroup FROM [TOneWhS_BE].[SupplierCode] sc WITH(NOLOCK)
	INNER JOIN [TOneWhS_BE].CodeGroup cg on cg.ID = sc.CodeGroupID
	WHERE ((@IsFuture = 0 AND sc.BED <= @EffectiveOn AND  (sc.EED > @EffectiveOn OR sc.EED IS NULL))
		   OR (@IsFuture = 1 AND (sc.BED > GETDATE() OR sc.EED IS NULL)))
	AND sc.Code = cg.Code 
END