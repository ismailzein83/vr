CREATE PROCEDURE [sec].[sp_View_GetById] 
	-- Add the parameters for the stored procedure here
@Id INT

AS
BEGIN
	SELECT	v.Id,
			v.Name,
			v.Title,
			v.Url,
			v.Module,
			v.[RequiredPermissions],
			v.[Audience],
			v.[Content],
			v.[Type],
			v.[Rank],
			m.Name ModuleName  
	FROM	sec.[View]
	AS		v 
	JOIN	sec.[Module] m 
	ON		v.Module=m.Id 
	WHERE	v.Id=@Id
END