-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_User_GetAll] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select	[ID], [Name], [Email], LastLogin, [Description], [TenantId] , [EnabledTill],ExtendedSettings, DisabledTill,Settings, IsSystemUser
	FROM	[sec].[User] WITH(NOLOCK)
	ORDER BY [Name]
END