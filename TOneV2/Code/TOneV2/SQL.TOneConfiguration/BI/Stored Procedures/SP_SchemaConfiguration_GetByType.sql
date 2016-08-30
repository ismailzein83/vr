CREATE procedure [BI].[SP_SchemaConfiguration_GetByType]
@Type INT
AS
BEGIN
	SELECT	sc.ID,sc.DisplayName,sc.Name,sc.[Type],sc.Configuration 
	FROM	BI.SchemaConfiguration sc  WITH(NOLOCK) 
	WHERE	sc.[Type]=@Type ORDER BY sc.[Rank]
END