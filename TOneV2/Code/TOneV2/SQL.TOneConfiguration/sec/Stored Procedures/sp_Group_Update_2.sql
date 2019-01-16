-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Group_Update]
	@ID int,
	@Name Nvarchar(255),
	@Description nvarchar(MAX),
	@Settings nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sec.[Group] WHERE ID != @ID AND Name = @Name)
	begin
		UPDATE [sec].[Group]
		SET Name = @Name,
			[Description] = @Description,
			[Settings] = @Settings,
			LastModifiedTime = GETDATE()
		WHERE ID = @ID
	end
END