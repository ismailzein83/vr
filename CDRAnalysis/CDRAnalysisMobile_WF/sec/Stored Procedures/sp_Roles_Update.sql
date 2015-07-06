-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Roles_Update]
	@ID int,
	@Name Nvarchar(255),
	@Description ntext
AS
BEGIN
	UPDATE [sec].[Role]
	SET Name = @Name,
		[Description] = @Description
	WHERE ID = @ID
END