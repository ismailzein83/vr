-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleCode_GetSpecificCodePrefixes]
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
	select ParsedString from [VR_NumberingPlan].[ParseStringList](@CodePrefixes);
	
	WITH AllPrefixes_CTE (CodePrefix, CodeCount)
	AS
	(
		SELECT	LEFT(sc.Code, @PrefixLength) as CodePrefix, Count(1) as CodeCount 
		FROM	[VR_NumberingPlan].[SaleCode] sc WITH (NOLOCK) 
		WHERE	((@IsFuture = 0 AND sc.BED <= @EffectiveOn AND (sc.EED > @EffectiveOn OR sc.EED IS NULL))OR(@IsFuture = 1 AND (sc.BED > GETDATE() OR sc.EED IS NULL)))
		group by LEFT(sc.Code, @PrefixLength)
	)

	SELECT	allPrefixes.CodePrefix, CodeCount
    FROM	AllPrefixes_CTE allPrefixes
			join @CodePrefixesTable cp on cp.CodePrefix = LEFT(allPrefixes.CodePrefix, @PrefixLength-1)

END