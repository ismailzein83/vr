﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Switch_Insert]
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail_BE.Switch
		WHERE Name = @Name
	)
	BEGIN
		INSERT INTO Retail_BE.Switch (Name,Settings)
		VALUES (@Name,@Settings)
		SET @ID = SCOPE_IDENTITY()
	END
END