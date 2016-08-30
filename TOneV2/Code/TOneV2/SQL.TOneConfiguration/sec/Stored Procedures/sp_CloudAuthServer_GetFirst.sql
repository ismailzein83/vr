-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_CloudAuthServer_GetFirst]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	TOP 1 [ID],[Settings],[ApplicationIdentification]
	FROM	[sec].[CloudAuthServer] WITH(NOLOCK) 
  
END