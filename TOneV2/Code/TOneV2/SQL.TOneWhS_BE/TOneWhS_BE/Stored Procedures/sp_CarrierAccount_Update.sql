-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_Update]
	@ID int,
	@Name nvarchar(255),
	@CarrierProfileId int,
	@AccountType int,
	@CustomerSettings nvarchar(MAX),
	@SupplierSettings nvarchar(MAX)
AS
BEGIN

	Update TOneWhS_BE.CarrierAccount
	Set Name = @Name,
		CarrierProfileID=@CarrierProfileId,
		AccountType=@AccountType,
		CustomerSettings=@CustomerSettings,
		SupplierSettings = @SupplierSettings
	Where ID = @ID
END