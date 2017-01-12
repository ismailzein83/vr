-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetDistinctCodePrefixes]
	@PrefixLength int,
	@EffectiveOn DATETIME,
	@IsFuture bit
AS
BEGIN

	Declare @PrefixLength_local int = @PrefixLength
	Declare @EffectiveOn_local DATETIME = @EffectiveOn
	Declare @IsFuture_local bit = @IsFuture

	SELECT LEFT(Code, @PrefixLength_local) as CodePrefix, SUM(1) as codeCount FROM TOneWhS_BE.SupplierCode WITH (NOLOCK) 
		WHERE
		(
			(@IsFuture_local = 0 AND BED <= @EffectiveOn_local AND (EED > @EffectiveOn_local OR EED IS NULL))
			OR
			(@IsFuture_local = 1 AND (BED > GETDATE() OR EED IS NULL))
		)
		group by LEFT(Code, @PrefixLength_local)
		order by codeCount desc;
	SET NOCOUNT OFF;
END