-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_Deal].[sp_Deal_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM TOneWhS_Deal.Deal WHERE [Name] = @Name and ID != @ID)
	BEGIN
		Update TOneWhS_Deal.Deal
		Set Settings=@Settings,Name=@Name
		Where ID = @ID
	END
END