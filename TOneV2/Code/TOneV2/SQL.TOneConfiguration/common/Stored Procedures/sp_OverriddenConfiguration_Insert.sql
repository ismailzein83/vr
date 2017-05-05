-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_OverriddenConfiguration_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@GroupId uniqueidentifier,
    @Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Common].OverriddenConfiguration WHERE Name = @Name)
	BEGIN
	INSERT INTO [Common].OverriddenConfiguration(ID,Name,GroupId,Settings)
	VALUES (@ID, @Name,@GroupId,@Settings)
	END
END