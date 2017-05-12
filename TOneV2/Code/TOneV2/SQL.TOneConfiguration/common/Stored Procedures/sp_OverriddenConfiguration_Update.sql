-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_OverriddenConfiguration_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@GroupId uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Common].OverriddenConfiguration WHERE ID != @ID and Name = @Name AND GroupId = @GroupId)
	BEGIN
		update [Common].OverriddenConfiguration
		set  Name = @Name ,Settings= @Settings, GroupId = @GroupId
		where  ID = @ID
	END
END