-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetSpecificCodePrefixes]
	@PrefixLength int,
	@CodePrefixes varchar(max),
	@EffectiveOn DATETIME,
	@IsFuture bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
    DECLARE @CodePrefixesTable TABLE (CodePrefix varchar(20))
    
	INSERT INTO @CodePrefixesTable (CodePrefix)
	select ParsedString from [TOneWhS_BE].[ParseStringList](@CodePrefixes)
	
	SET NOCOUNT ON;
		SELECT LEFT(Code, @PrefixLength) as CodePrefix, SUM(1) as codeCount 
		FROM TOneWhS_BE.SupplierCode sc WITH (NOLOCK) 
		join @CodePrefixesTable cp on cp.CodePrefix = LEFT(sc.Code, @PrefixLength-1)
		WHERE
		(
			(@IsFuture = 0 AND BED <= @EffectiveOn AND (EED > @EffectiveOn OR EED IS NULL))
			OR
			(@IsFuture = 1 AND (BED > GETDATE() OR EED IS NULL))
		)
		group by LEFT(Code, @PrefixLength)
	SET NOCOUNT OFF;
END