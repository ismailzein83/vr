-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_CarrierProfile_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN

	Update TOneWhS_BE.CarrierProfile
	Set Name = @Name,
		Settings=@Settings
	Where ID = @ID
END