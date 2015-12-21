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
	Insert into TOneWhS_BE.SupplierPriceList(SupplierID,CurrencyID)
	Values(@SupplierAccountId,@CurrencyId)
	Set @Id = @@IDENTITY
END