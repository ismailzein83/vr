-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountPartDefinition_Insert]
	@Name NVARCHAR(255),
	@Title NVARCHAR(255),
	@Details NVARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail_BE.AccountPartDefinition
		WHERE Name = @Name
	)
	BEGIN
		INSERT INTO Retail_BE.AccountPartDefinition (Name, Title, Details)
		VALUES (@Name, @Title, @Details)
		SET @ID = @@IDENTITY
	END
END