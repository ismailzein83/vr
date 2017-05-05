-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Common].[sp_GenericLKUP_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Common].GenericLKUP WHERE ID != @ID and Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
		update [Common].GenericLKUP
		set  Name = @Name ,Settings= @Settings, BusinessEntityDefinitionID = @BusinessEntityDefinitionId
		where  ID = @ID
	END
END