
CREATE PROCEDURE [dbo].[sp_Zone_GetIds] 
	@EffectiveOn DATETIME,
	@IsEffectiveInFuture bit
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [ID]
	FROM [dbo].[Zone] sz
	WHERE 
		(
			(@IsEffectiveInFuture = 0 AND sz.BED <= @EffectiveOn AND (sz.EED > @EffectiveOn OR sz.EED IS NULL))
			OR
			(@IsEffectiveInFuture = 1 AND (sz.BED > GETDATE() OR sz.EED IS NULL))
		);
END