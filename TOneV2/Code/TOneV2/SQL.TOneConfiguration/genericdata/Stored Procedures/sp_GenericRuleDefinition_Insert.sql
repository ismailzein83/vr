-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_GenericRuleDefinition_Insert]
	@Name NVARCHAR(900),
	@Details NVARCHAR(MAX),
	@Id INT OUT
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM genericdata.GenericRuleDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.GenericRuleDefinition (Name, Details, CreatedTime)
		VALUES (@Name, @Details, GETDATE())
		SET @Id = SCOPE_IDENTITY()
	END
END