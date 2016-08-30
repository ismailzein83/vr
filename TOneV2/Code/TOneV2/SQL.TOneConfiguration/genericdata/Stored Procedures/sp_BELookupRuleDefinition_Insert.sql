-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BELookupRuleDefinition_Insert]
	@Name NVARCHAR(450),
	@Details NVARCHAR(MAX),
	@ID INT	OUT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM genericdata.BELookupRuleDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.BELookupRuleDefinition (Name, Details)
		VALUES (@Name, @Details)
		SET @ID = SCOPE_IDENTITY()
	END
END