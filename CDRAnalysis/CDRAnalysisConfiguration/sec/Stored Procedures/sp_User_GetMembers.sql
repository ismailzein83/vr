-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_User_GetMembers] 
	@RoleId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select [ID], [Name], [Password], [Email], [Status], [LastLogin], [Description] FROM sec.[User] as A
	Join sec.UserRole as B on A.ID = B.UserID
	where B.RoleID = @RoleId
END