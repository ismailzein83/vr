-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_ActionDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX),
	@EntityType int

AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Retail_BE.ActionDefinition WHERE Name = @Name And EntityType = @EntityType)
	BEGIN
		INSERT INTO Retail_BE.ActionDefinition (ID,Name, Settings,EntityType)
		VALUES (@ID,@Name, @Settings,@EntityType)
	END
END