

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
	SELECT	v.[Id],v.[Name],v.[Title],v.[Url],v.[Module],v.[ActionNames],v.[Audience],v.[Type],v.[Content],v.[Settings],v.[Rank]
	FROM	[sec].[View] v WITH(NOLOCK) 
    ORDER BY v.[Module],v.[Rank]
END