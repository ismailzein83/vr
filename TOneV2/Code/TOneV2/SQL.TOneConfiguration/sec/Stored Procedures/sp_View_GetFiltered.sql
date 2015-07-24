-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_View_GetFiltered]
	-- Add the parameters for the stored procedure here
@Filter nvarchar(255) ,
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
	WHERE	v.[Type]=@Type and (v.Name Like '%'+@Filter+'%' or @Filter IS NULL)
END