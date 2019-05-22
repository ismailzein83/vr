-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_GenericRuleDefinition_Insert]
	@Id uniqueidentifier ,
	@Name NVARCHAR(900),
	@DevProjectId uniqueidentifier,
	@Details NVARCHAR(MAX)

AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM genericdata.GenericRuleDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.GenericRuleDefinition (ID,Name,DevProjectId,Details, CreatedTime)
		VALUES (@Id,@Name,@DevProjectId, @Details, GETDATE())
	
	END
END