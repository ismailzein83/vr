CREATE procedure [Analytic].[SP_SchemaConfiguration_GetByType]
@Type INT
AS
BEGIN
	SELECT	sc.ID,sc.Name,sc.DisplayName,sc.[Type],sc.Configuration 
	FROM	Analytic.SchemaConfiguration sc  WITH(NOLOCK) 
	WHERE	sc.[Type]=@Type
END