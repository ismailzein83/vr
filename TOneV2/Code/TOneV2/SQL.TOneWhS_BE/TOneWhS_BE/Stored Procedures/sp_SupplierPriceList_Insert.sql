-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierPriceList_Insert]
	@SupplierAccountId int,
	@CurrencyId int,
	@Id int out
AS
BEGIN
	Insert into TOneWhS_BE.SupplierPriceList(SupplierID,CurrencyID,CreatedTime)
	Values(@SupplierAccountId,@CurrencyId,GETDATE())
	Set @Id = SCOPE_IDENTITY()
END