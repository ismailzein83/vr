-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierProfile_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(select 1 from TOneWhS_BE.CarrierProfile where Name = @Name and Id!=@ID and ISNULL(IsDeleted,0) = 0) 
BEGIN
	Update TOneWhS_BE.CarrierProfile
	Set Name = @Name,
		Settings=@Settings,
		LastModifiedBy=@LastModifiedBy,
		LastModifiedTime=GETDATE()
	Where ID = @ID
END
END