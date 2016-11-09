-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountPartDefinition_Insert]
	@ID uniqueidentifier ,
	@Name NVARCHAR(255),
	@Title NVARCHAR(255),
	@Details NVARCHAR(MAX)

AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail_BE.AccountPartDefinition
		WHERE Name = @Name
	)
	BEGIN
		INSERT INTO Retail_BE.AccountPartDefinition (ID,Name, Title, Details)
		VALUES (@ID,@Name, @Title, @Details)

	END
END