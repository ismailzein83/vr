-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountPartDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Title NVARCHAR(255),
	@Details NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail_BE.AccountPartDefinition
		WHERE ID != @ID AND Name = @Name
	)
	BEGIN
		UPDATE Retail_BE.AccountPartDefinition
		SET Name = @Name, Title = @Title, Details = @Details
		WHERE ID = @ID
	END
END