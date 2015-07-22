
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Module_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  [Id]
      ,[Name]
      ,[Title]
      ,[Url]
      ,[ParentId]
      ,[Icon]
      ,[AllowDynamic] from sec.Module
      ORDER BY [Rank]
END