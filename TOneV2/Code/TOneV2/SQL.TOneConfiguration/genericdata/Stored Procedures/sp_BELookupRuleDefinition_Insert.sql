-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BELookupRuleDefinition_Insert]
	@ID uniqueidentifier	,
	@Name NVARCHAR(450),
	@Details NVARCHAR(MAX)

AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM genericdata.BELookupRuleDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.BELookupRuleDefinition (ID,Name, Details)
		VALUES (@ID,@Name, @Details)
	END
END