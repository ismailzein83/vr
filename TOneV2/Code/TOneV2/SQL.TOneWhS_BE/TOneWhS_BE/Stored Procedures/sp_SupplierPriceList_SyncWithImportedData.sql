-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierPriceList_SyncWithImportedData]
	@PriceListID int,
	@SupplierID int,
	@CurrencyID int
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRAN

    Insert into Tonewhs_be.SupplierPriceList (ID, SupplierID, CurrencyID)
	values(@PriceListID, @SupplierID, @CurrencyID)
	
	Insert into TOneWhS_BE.SupplierZone (ID, CountryID, Name, SupplierID, BED, EED)
	Select sznew.ID, sznew.CountryID, sznew.Name, sznew.SupplierID, sznew.BED, sznew.EED 
	from TOneWhS_BE.SPL_SupplierZone_New sznew Where sznew.PriceListID = @PriceListID AND sznew.SupplierID = @SupplierID 
	
	Insert into TOneWhS_BE.SupplierCode (ID, Code, ZoneID, CodeGroupID, BED, EED)
	Select scnew.ID, scnew.Code, scnew.ZoneID, scnew.CodeGroupID, scnew.BED, scnew.EED
	from TOneWhS_BE.SPL_SupplierCode_New scnew Where scnew.PriceListID = @PriceListID
	
	Insert into TOnewhs_BE.SupplierRate (ID, PriceListID, ZoneID, CurrencyID, NormalRate, OtherRates, BED, EED)
	Select srnew.ID, srnew.PriceListID, srnew.ZoneID, srnew.CurrencyID, srnew.NormalRate, srnew.OtherRates, srnew.BED, srnew.EED
	from TOneWhS_BE.SPL_SupplierRate_New srnew Where srnew.PriceListID = @PriceListID
	
	Update ToneWhs_be.SupplierZone
	Set EED = szchanged.EED
	from ToneWhs_be.SupplierZone sz join TOneWhS_BE.SPL_SupplierZone_Changed szchanged
	on sz.ID = szchanged.ID Where szchanged.PriceListID = @PriceListID
	
	Update TOneWhs_BE.SupplierCode
	Set EED = scchanged.EED
	from TOneWhs_BE.SupplierCode sc join TOneWhS_BE.SPL_SupplierCode_Changed scchanged
	on sc.ID = scchanged.ID Where scchanged.PriceListID = @PriceListID
	
	Update TOneWhs_BE.SupplierRate
	Set EED = srchanged.EED
	from TOneWhs_BE.SupplierRate sr join TOneWhS_BE.SPL_SupplierRate_Changed srchanged
	on sr.ID = srchanged.ID Where srchanged.PriceListID = @PriceListID
	
	COMMIT TRAN
END