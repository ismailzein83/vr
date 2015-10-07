-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_Insert]
	@Name NVARCHAR(255),
	@Symbol NVARCHAR(50),
	@SwitchID INT,
	@Type INT,
	@Direction INT,
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.SwitchTrunk WHERE Name = @Name OR Symbol = @Symbol)
	BEGIN
		INSERT INTO PSTN_BE.SwitchTrunk (Name, Symbol, SwitchID, [Type], Direction)
		VALUES (@Name, @Symbol, @SwitchID, @Type, @Direction)
		
		SET @ID = @@IDENTITY
	END
END