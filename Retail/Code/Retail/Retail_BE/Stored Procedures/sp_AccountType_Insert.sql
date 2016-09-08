-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountType_Insert]
	@Name NVARCHAR(255),
	@Title NVARCHAR(255),
	@Settings NVARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Retail_BE.AccountType WHERE Name = @Name)
	BEGIN
		INSERT INTO Retail_BE.AccountType (Name, Title, Settings)
		VALUES (@Name, @Title, @Settings)
		SET @ID = SCOPE_IDENTITY()
	END
END