-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_Update]
	@ID int,
	@Name nvarchar(255),
	@CarrierProfileId INT,
	@SellingProductId int = null,
	@CustomerSettings nvarchar(MAX),
	@SupplierSettings nvarchar(MAX),
	@CarrierAccountSettings nvarchar(MAX),
	@LastModifiedBy int
	
AS
BEGIN
IF NOT EXISTS(select 1 from TOneWhS_BE.CarrierAccount WHERE [NameSuffix] = @Name and [CarrierProfileID] = @CarrierProfileId and Id!=@ID and ISNULL(IsDeleted,0) = 0) 
BEGIN
	Update TOneWhS_BE.CarrierAccount
	Set NameSuffix = @Name,
		SellingProductID = @SellingProductId,
		CustomerSettings=@CustomerSettings,
		SupplierSettings = @SupplierSettings,
		CarrierAccountSettings=@CarrierAccountSettings,
		LastModifiedBy=@LastModifiedBy,
		LastModifiedTime=GETDATE()
	Where ID = @ID
END

END