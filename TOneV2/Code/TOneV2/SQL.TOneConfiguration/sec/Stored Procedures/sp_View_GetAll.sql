

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
	SELECT [Id]
      ,[Name]
      ,[Title]
      ,[Url]
      ,[Module]
      ,[RequiredPermissions]
      ,[Audience]
      ,[Type]
      FROM [sec].[View]
      ORDER BY [Module],[Rank]
END