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
	Declare @PrefixLength_local int = @PrefixLength
	Declare @EffectiveOn_local DATETIME = @EffectiveOn
	Declare @IsFuture_local bit = @IsFuture

    DECLARE @CodePrefixesTable TABLE (CodePrefix varchar(20))
	INSERT INTO @CodePrefixesTable (CodePrefix)
	select ParsedString from [TOneWhS_BE].[ParseStringList](@CodePrefixes);
	
	WITH AllPrefixes_CTE (CodePrefix, CodeCount)
	AS
	(
		SELECT LEFT(Code, @PrefixLength_local) as CodePrefix, Count(1) as CodeCount 
		FROM TOneWhS_BE.SupplierCode sc WITH (NOLOCK) 
		WHERE((@IsFuture_local = 0 AND BED <= @EffectiveOn_local AND (EED > @EffectiveOn_local OR EED IS NULL))OR(@IsFuture_local = 1 AND (BED > GETDATE() OR EED IS NULL)))
		group by LEFT(Code, @PrefixLength_local)
	)

	SELECT allPrefixes.CodePrefix, CodeCount
    FROM AllPrefixes_CTE allPrefixes
    join @CodePrefixesTable cp on cp.CodePrefix = LEFT(allPrefixes.CodePrefix, @PrefixLength_local-1)
END