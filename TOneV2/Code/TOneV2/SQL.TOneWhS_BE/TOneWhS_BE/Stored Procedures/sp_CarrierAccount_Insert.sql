-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_Insert]
	@Name nvarchar(255),
	@CarrierProfileId INT,
	@AccountType int,
	@CustomerSettings nvarchar(MAX),
	@SupplierSettings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into TOneWhS_BE.CarrierAccount([Name],[CarrierProfileID],[AccountType],[SupplierSettings] ,[CustomerSettings] )
	Values(@Name,@CarrierProfileId, @AccountType, @SupplierSettings,@CustomerSettings)
	
	Set @Id = @@IDENTITY
END