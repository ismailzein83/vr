-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SellingProduct_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(MAX), 
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(select 1 from TOneWhS_BE.SellingProduct where Name = @Name and Id!=@ID) 
BEGIN
	Update TOneWhS_BE.SellingProduct
	Set Name = @Name,
		Settings = @Settings,
		LastModifiedBy = @LastModifiedBy,
		LastModifiedTime = GETDATE()
	Where ID = @ID
	END
END