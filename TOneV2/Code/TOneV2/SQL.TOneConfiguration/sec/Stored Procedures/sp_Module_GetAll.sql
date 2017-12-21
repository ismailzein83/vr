
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
	SELECT  [Id],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[AllowDynamic],[Rank],[Settings]
	from	[sec].Module WITH(NOLOCK) 
    ORDER BY [Rank]
END