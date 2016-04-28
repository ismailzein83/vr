-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [cloud].[sp_CloudApplicationUser_Insert]
	@ApplicationId int,
	@UserId int,
	@Settings nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	IF NOT EXISTS (SELECT NULL FROM cloud.CloudApplicationUser WHERE ApplicationID = @ApplicationId AND UserID = @UserId)
	BEGIN
		INSERT INTO [cloud].[CloudApplicationUser]
			   ([ApplicationID]
			   ,[UserID]
			   ,[Settings])
		 VALUES
			   (@ApplicationId
			   ,@UserId
			   ,@Settings)
    END
END