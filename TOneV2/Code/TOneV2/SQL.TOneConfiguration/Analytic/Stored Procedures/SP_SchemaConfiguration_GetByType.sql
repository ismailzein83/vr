CREATE procedure [Analytic].[SP_SchemaConfiguration_GetByType]
@Type INT
AS
BEGIN
	SELECT	sc.ID,
			sc.Name,
			sc.DisplayName,
			sc.[Type],
			sc.Configuration 
	FROM	Analytic.SchemaConfiguration sc 
	WHERE sc.[Type]=@Type
END