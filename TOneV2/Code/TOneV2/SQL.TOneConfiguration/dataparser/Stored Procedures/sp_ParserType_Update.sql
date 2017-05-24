-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dataparser].[sp_ParserType_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.VRComponentType WHERE ID != @ID and Name = @Name)
	BEGIN
		update dataparser.ParserType
		set  Name = @Name, Settings = @Settings
		where  ID = @ID
	END
END