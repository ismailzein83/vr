CREATE PROCEDURE [BEntity].[sp_Code_GetDistinctCodePrefixes]
	@PrefixLength int,
	@EffectiveTime DATETIME,
	@IsFuture bit
AS
BEGIN
  
SELECT DISTINCT LEFT(c.Code, @PrefixLength) CodePrefix INTO #DistinctCodePrefix FROM Code c WITH (NOLOCK) 
WHERE c.Code not in ('2', '3', '4', '5', '6', '8', '9')
		AND 
		(
			(@IsFuture = 0 AND c.BeginEffectiveDate <= @EffectiveTime AND (c.EndEffectiveDate > @EffectiveTime OR c.EndEffectiveDate IS NULL))
			OR
			(@IsFuture = 1 AND (c.BeginEffectiveDate > GETDATE() OR c.EndEffectiveDate IS NULL))
		);
 
WITH ValidCG AS (SELECT DISTINCT cg1.CodePrefix FROM #DistinctCodePrefix cg1 
				 LEFT JOIN #DistinctCodePrefix cg2 ON cg1.CodePrefix <> cg2.CodePrefix AND cg1.CodePrefix like cg2.CodePrefix +'%'
				 WHERE cg2.CodePrefix IS NULL)
 
SELECT CodePrefix FROM ValidCG ORDER BY CodePrefix

DROP TABLE #DistinctCodePrefix 

END