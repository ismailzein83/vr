-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_ActionDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX),
	@EntityType int
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Retail_BE.ActionDefinition WHERE Name = @Name AND ID != @ID And EntityType = @EntityType)
	BEGIN
		UPDATE Retail_BE.ActionDefinition
		SET Name = @Name, Settings = @Settings, EntityType = @EntityType
		WHERE ID = @ID
	END
END