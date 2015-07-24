CREATE procedure [BI].[SP_SchemaConfiguration_GetByType]
@Type INT
AS
BEGIN
	SELECT	sc.ID,
			sc.DisplayName,
			sc.Name,sc.[Type],
			sc.Configuration 
	FROM	BI.SchemaConfiguration sc 
	WHERE sc.[Type]=@Type;
END