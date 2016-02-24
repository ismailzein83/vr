-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorAccount_Update]
	@ID int,
	@Name nvarchar(255),
	@OperatorProfileId int,
	@CustomerSettings nvarchar(MAX),
	@SupplierSettings nvarchar(MAX),
	@OperatorAccountSettings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(select 1 from dbo.OperatorAccount where NameSuffix = @Name and Id!=@ID) 
BEGIN
	Update dbo.OperatorAccount
	Set NameSuffix = @Name,
		OperatorProfileID=@OperatorProfileId,
		CustomerSettings=@CustomerSettings,
		SupplierSettings = @SupplierSettings,
		OperatorAccountSettings=@OperatorAccountSettings
		
	Where ID = @ID
END

END