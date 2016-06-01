-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Retail.sp_Account_Update
	@ID INT,
	@Name NVARCHAR(255),
	@Type INT,
	@Settings NVARCHAR(MAX),
	@ParentID INT
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Retail.Account WHERE ID != @ID AND Name = @Name AND ParentID = @ParentID)
	BEGIN
		UPDATE Retail.Account
		SET Name = @Name, [Type] = @Type, Settings = @Settings
		WHERE ID = @ID
	END
END