-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_Insert]
	@Name NVARCHAR(255),
	@TypeID INT = NULL,
	@AreaCode VARCHAR(10) = NULL,
	@TimeOffset VARCHAR(50) = NULL,
	@InsertedID INT OUT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.Switch WHERE Name = @Name)
	BEGIN
		INSERT INTO PSTN_BE.Switch (Name, TypeID, AreaCode, TimeOffset)
		VALUES (@Name, @TypeID, @AreaCode, @TimeOffset)
		
		SET @InsertedID = @@IDENTITY
	END
END