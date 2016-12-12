-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [logging].[sp_UserActionAudit_Insert]
	@UserId INT ,
	@ModuleName varchar(50),
	@ControllerName  varchar(50),
	@ActionName varchar (100),
	@BaseUrl varchar(100)
AS
BEGIN
	INSERT INTO logging.UserActionAudit(UserID,ModuleName,ControllerName,ActionName,BaseUrl,LogTime)
	VALUES (@UserId,@ModuleName,@ControllerName,@ActionName,@BaseUrl,GETDATE())

END