CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZone_GetIds] 
	@EffectiveOn DATETIME,
	@IsEffectiveInFuture bit
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [ID]
	FROM [TOneWhS_BE].[SaleZone] sz with(nolock)
	WHERE 
		(
			(@IsEffectiveInFuture = 0 AND sz.BED <= @EffectiveOn AND (sz.EED > @EffectiveOn OR sz.EED IS NULL))
			OR
			(@IsEffectiveInFuture = 1 AND (sz.BED > GETDATE() OR sz.EED IS NULL))
		);
END