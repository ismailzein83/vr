Create procedure Analytic.[SP_SchemaConfiguration_GetByType]
@Type INT
AS
BEGIN
	SELECT	sc.ID,
			sc.Name,
			sc.[Type],
			sc.Configuration 
	FROM	Analytic.SchemaConfiguration sc 
	WHERE sc.[Type]=@Type
END