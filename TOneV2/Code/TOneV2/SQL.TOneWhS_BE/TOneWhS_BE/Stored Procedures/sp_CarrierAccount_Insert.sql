-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_Insert]
	@Name nvarchar(255),
	@CarrierProfileId INT,
	@AccountType int,
	@SellingNumberPlanID int = NULL,
	@CustomerSettings nvarchar(MAX),
	@SupplierSettings nvarchar(MAX),
	@CarrierAccountSettings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into TOneWhS_BE.CarrierAccount([Name],[CarrierProfileID],[AccountType],[SupplierSettings] ,[CustomerSettings],[CarrierAccountSettings],[SellingNumberPlanID] )
	Values(@Name,@CarrierProfileId, @AccountType, @SupplierSettings,@CustomerSettings,@CarrierAccountSettings,@SellingNumberPlanID)
	
	Set @Id = @@IDENTITY
END