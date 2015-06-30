CREATE PROCEDURE [sec].[sp_View_GetById] 
	-- Add the parameters for the stored procedure here
@Id int

AS
BEGIN
	SELECT v.Id,v.Name,v.Url,v.Module,v.[RequiredPermissions],v.[Audience],v.[Content],v.[Type] from sec.[View]as v where Id=@Id
END