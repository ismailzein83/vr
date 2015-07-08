CREATE PROCEDURE [sec].[sp_View_GetByType]
@Type INT
AS
BEGIN
	SELECT	v.Id,
			v.Name PageName,
			v.Module ModuleId,
			v.Url,v.Audience,
			v.Content,
			v.[Type],
			m.Name ModuleName 
	FROM	sec.[View] v 
	INNER JOIN	sec.[Module] m 
	ON		v.Module=m.Id 
	WHERE	v.[Type]=@Type
END