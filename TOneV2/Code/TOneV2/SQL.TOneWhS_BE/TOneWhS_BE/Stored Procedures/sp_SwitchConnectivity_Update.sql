-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchConnectivity_Update]
	@ID int,
	@Name nvarchar(450),
	@SwitchID int,
	@CarrierAccountID int,
	@Settings nvarchar(MAX),
	@BED Datetime,
	@EED Datetime,
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(select 1 from TOneWhS_BE.SwitchConnectivity WHERE [Name] = @Name and Id!=@ID) 
BEGIN
	Update TOneWhS_BE.SwitchConnectivity
	Set Name = @Name,
	    CarrierAccountID =@CarrierAccountID,
		Settings=@Settings,
		SwitchID = @SwitchID,
		BED=@BED,
		EED=@EED,
		LastModifiedBy = @LastModifiedBy,
		LastModifiedTime = GETDATE()
	Where ID = @ID
END

END