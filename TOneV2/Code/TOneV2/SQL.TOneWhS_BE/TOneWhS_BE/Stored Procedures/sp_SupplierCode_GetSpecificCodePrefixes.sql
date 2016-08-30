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
	select ParsedString from [TOneWhS_BE].[ParseStringList](@CodePrefixes);
	
	WITH AllPrefixes_CTE (CodePrefix, CodeCount)
	AS
	(
		SELECT LEFT(Code, @PrefixLength) as CodePrefix, Count(1) as CodeCount 
		FROM TOneWhS_BE.SupplierCode sc WITH (NOLOCK) 
		WHERE((@IsFuture = 0 AND BED <= @EffectiveOn AND (EED > @EffectiveOn OR EED IS NULL))OR(@IsFuture = 1 AND (BED > GETDATE() OR EED IS NULL)))
		group by LEFT(Code, @PrefixLength)
	)

	SELECT allPrefixes.CodePrefix, CodeCount
    FROM AllPrefixes_CTE allPrefixes
    join @CodePrefixesTable cp on cp.CodePrefix = LEFT(allPrefixes.CodePrefix, @PrefixLength-1)
END