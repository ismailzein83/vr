-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Common].[sp_GenericLKUP_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
    @Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Common].GenericLKUP WHERE Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
	INSERT INTO [Common].[GenericLKUP] (ID,Name,BusinessEntityDefinitionID,Settings)
	VALUES (@ID, @Name,@BusinessEntityDefinitionId,@Settings)
	END
END