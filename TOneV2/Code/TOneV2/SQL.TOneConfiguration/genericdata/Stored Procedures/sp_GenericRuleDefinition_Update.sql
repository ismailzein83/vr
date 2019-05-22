-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_GenericRuleDefinition_Update]
	@Id uniqueidentifier,
	@Name NVARCHAR(900),
	@DevProjectId uniqueidentifier,
	@Details NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM genericdata.GenericRuleDefinition WHERE ID != @Id AND Name = @Name)
	BEGIN
		UPDATE genericdata.GenericRuleDefinition
		SET Name = @Name,DevProjectID=@DevProjectId, Details = @Details, LastModifiedTime = GETDATE()
		WHERE ID = @Id
	END
END