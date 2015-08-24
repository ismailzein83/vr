

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_View_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT v.[Id]
      ,v.[Name]
      ,v.[Title]
      ,v.[Url]
      ,v.[Module]
      ,v.[RequiredPermissions]
      ,v.[Audience]
      ,v.[Type]
      ,v.[Content]
      ,v.[Rank]
      ,m.Name as ModuleName
      FROM [sec].[View] v
      LEFT JOIN	sec.[Module] m 
	  ON 	v.Module=m.Id 
      ORDER BY v.[Module],v.[Rank]
END