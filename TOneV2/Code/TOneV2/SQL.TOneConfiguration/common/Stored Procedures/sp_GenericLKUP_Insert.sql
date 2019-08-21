-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_GenericLKUP_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
    @Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [common].[GenericLKUP] WHERE Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
	INSERT INTO [common].[GenericLKUP] (ID,Name,BusinessEntityDefinitionID,Settings)
	VALUES (@ID, @Name,@BusinessEntityDefinitionId,@Settings)
	END
END