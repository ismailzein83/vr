-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BELookupRuleDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(450),
	@Details NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM genericdata.BELookupRuleDefinition WHERE Name = @Name AND ID != @ID)
	BEGIN
		UPDATE genericdata.BELookupRuleDefinition
		SET Name = @Name, Details = @Details
		WHERE ID = @ID
	END
END