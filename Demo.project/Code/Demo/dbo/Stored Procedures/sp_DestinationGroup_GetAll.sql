




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_DestinationGroup_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT grp.[ID]
		  ,grp.[DestinationType]
		  ,grp.[GroupSettings]
		  ,grp.[Name]

	FROM	[dbo].DestinationGroup  as grp WITH(NOLOCK) 
END