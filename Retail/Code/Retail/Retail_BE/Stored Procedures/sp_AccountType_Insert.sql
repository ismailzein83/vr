﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountType_Insert]
	@ID uniqueidentifier ,
	@Name NVARCHAR(255),
	@Title NVARCHAR(255),
	@Settings NVARCHAR(MAX)

AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Retail_BE.AccountType WHERE Name = @Name)
	BEGIN
		INSERT INTO Retail_BE.AccountType (ID,Name, Title, Settings)
		VALUES (@ID,@Name, @Title, @Settings)

	END
END