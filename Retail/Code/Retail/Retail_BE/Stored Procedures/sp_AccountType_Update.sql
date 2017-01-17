-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountType_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Title NVARCHAR(255),
	@AccountBEDefinitionId uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Retail_BE.AccountType WHERE Name = @Name AND ID != @ID)
	BEGIN
		UPDATE Retail_BE.AccountType
		SET Name = @Name, Title = @Title, Settings = @Settings,AccountBEDefinitionId=@AccountBEDefinitionId
		WHERE ID = @ID
	END
END