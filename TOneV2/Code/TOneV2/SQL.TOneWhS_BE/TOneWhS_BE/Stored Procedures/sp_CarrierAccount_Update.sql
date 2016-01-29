-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_Update]
	@ID int,
	@Name nvarchar(255),
	@CarrierProfileId int,
	@AccountType int,
	@CustomerSettings nvarchar(MAX),
	@SupplierSettings nvarchar(MAX),
	@CarrierAccountSettings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(select 1 from TOneWhS_BE.CarrierProfile where Name = @Name and Id!=@ID) 
BEGIN
	Update TOneWhS_BE.CarrierAccount
	Set NameSuffix = @Name,
		CarrierProfileID=@CarrierProfileId,
		AccountType=@AccountType,
		CustomerSettings=@CustomerSettings,
		SupplierSettings = @SupplierSettings,
		CarrierAccountSettings=@CarrierAccountSettings
		
	Where ID = @ID
END

END