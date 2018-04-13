-- =============================================
-- Description:	Adding try catch will rollback.
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_SyncWithImportedData_SubProcedure]
	@ProcessInstanceID Bigint,
	@SellingNumberPlanId int,
	@PricelistStateBackupID Bigint
AS
BEGIN
	SET NOCOUNT ON;

Begin Try
	BEGIN TRAN
	
	Insert into TOneWhS_BE.SalePriceList (ID, OwnerType, OwnerID, CurrencyID, EffectiveOn, ProcessInstanceID,FileID, PriceListType, UserID,[Description],PricelistStateBackupID,PricelistSource)
	Select	splnew.ID, splnew.OwnerType, splnew.OwnerID, splnew.CurrencyID, splnew.EffectiveOn, @ProcessInstanceID,splnew.FileID, splnew.PriceListType,UserID,splnew.[Description],@PricelistStateBackupID,1
	from TOneWhS_BE.SalePriceList_new splnew WITH(NOLOCK) Where splnew.ProcessInstanceID = @ProcessInstanceID 
	
	Insert into TOneWhS_BE.SaleZone (ID,SellingNumberPlanID, CountryID, Name, BED, EED,ProcessInstanceID)
	Select sznew.ID,sznew.SellingNumberPlanID, sznew.CountryID, sznew.Name, sznew.BED, sznew.EED ,@ProcessInstanceID
	from TOneWhS_BE.CP_SaleZone_New sznew WITH(NOLOCK) Where sznew.ProcessInstanceID = @ProcessInstanceID AND sznew.SellingNumberPlanID = @SellingNumberPlanId 
	
	Insert into TOneWhS_BE.SaleCode (ID, Code, ZoneID, CodeGroupID, BED, EED,ProcessInstanceID)
	Select scnew.ID, scnew.Code, scnew.ZoneID, scnew.CodeGroupID, scnew.BED, scnew.EED,@ProcessInstanceID
	from TOneWhS_BE.CP_SaleCode_New scnew WITH(NOLOCK) Where scnew.ProcessInstanceID = @ProcessInstanceID

	Insert into TOneWhS_BE.SaleRate (ID, PriceListID, ZoneID, CurrencyID, Rate, Change, BED, EED)
	select newRate.ID, newRate.PriceListID, newRate.ZoneID, newRate.CurrencyId, newRate.NormalRate, 1, newRate.BED, newRate.EED
	from TOneWhS_BE.CP_SaleRate_New newRate WITH(NOLOCK) where newRate.ProcessInstanceID = @ProcessInstanceId

	Insert into TOneWhS_BE.SaleEntityRoutingProduct(ID, OwnerType,OwnerID, ZoneID, RoutingProductID, BED, EED)
	select newZoneRP.ID, newZoneRP.OwnerType, newZoneRP.OwnerID, newZoneRP.ZoneID, newZoneRP.RoutingProductID, newZoneRP.BED, newZoneRP.EED
	from TOneWhS_BE.CP_SaleZoneRoutingProduct_New newZoneRP WITH(NOLOCK) where newZoneRP.ProcessInstanceID = @ProcessInstanceId

	INSERT INTO [TOneWhS_BE].[SalePricelistCodeChange] ([Code],[RecentZoneName],[ZoneName],[ZoneID],[Change],[BatchID],[BED],[EED],[CountryID])
    select  spcc.[Code],spcc.[RecentZoneName],spcc.[ZoneName],spcc.[ZoneID],spcc.[Change],spcc.[BatchID],spcc.BED,spcc.EED,spcc.CountryID
	from  [TOneWhS_BE].[SalePricelistCodeChange_New] spcc with(nolock) where spcc.[BatchID] = @ProcessInstanceID

	INSERT INTO [TOneWhS_BE].[SalePricelistCustomerChange] ([BatchID],[PricelistID],[CountryID],[CustomerID])
	select [BatchID],[PricelistID],[CountryID],[CustomerID]
	from  [TOneWhS_BE].[SalePricelistCustomerChange_New] spcc with(nolock) where spcc.[BatchID]= @ProcessInstanceID

	INSERT INTO [TOneWhS_BE].[SalePricelistRateChange] ([PricelistId],[Rate],[RateTypeId],[RecentRate],[CountryID],[ZoneName],[Change],[BED],[EED],[RoutingProductID],[CurrencyID],ZoneID)
	select sprc.[PricelistId],sprc.[Rate],sprc.[RateTypeId],sprc.[RecentRate],sprc.[CountryID],sprc.[ZoneName],sprc.[Change],sprc.[BED],sprc.[EED],sprc.RoutingProductID,sprc.CurrencyID,sprc.ZoneID
	from [TOneWhS_BE].[SalePricelistRateChange_New] sprc with(nolock)
	where sprc.ProcessInstanceID = @ProcessInstanceID

	INSERT INTO [TOneWhS_BE].[SalePricelistRPChange]  ([ZoneName],[ZoneID],[RoutingProductId],[RecentRoutingProductId],[BED],[EED],[PriceListId],[CountryId], [CustomerId])
	SELECT sprpc.[ZoneName],sprpc.[ZoneID],sprpc.[RoutingProductId],sprpc.[RecentRoutingProductId],sprpc.[BED],sprpc.[EED],sprpc.[PriceListId],sprpc.[CountryId], sprpc.CustomerId
	FROM [TOneWhS_BE].[SalePricelistRPChange_New] sprpc with(nolock)
	where sprpc.ProcessInstanceID = @ProcessInstanceID

	Update ToneWhs_be.SaleZone
	Set EED = szchanged.EED
	from ToneWhs_be.SaleZone sz join TOneWhS_BE.CP_SaleZone_Changed szchanged with(nolock)
	on sz.ID = szchanged.ID Where szchanged.ProcessInstanceID = @ProcessInstanceID
	
	Update TOneWhs_BE.SaleCode
	Set EED = scchanged.EED
	from TOneWhs_BE.SaleCode sc join TOneWhS_BE.CP_SaleCode_Changed scchanged with(nolock)
	on sc.ID = scchanged.ID Where scchanged.ProcessInstanceID = @ProcessInstanceID
	
	Update TOneWhs_BE.CustomerCountry
	Set EED = scchanged.EED
	from TOneWhs_BE.CustomerCountry sc join TOneWhS_BE.CP_CustomerCountry_Changed scchanged with(nolock)
	on sc.ID = scchanged.ID Where scchanged.ProcessInstanceID = @ProcessInstanceID

	Update TOneWhs_BE.SaleRate
	Set EED = srchanged.EED
	from TOneWhs_BE.SaleRate sr join TOneWhS_BE.CP_SaleRate_Changed srchanged with(nolock)
	on sr.ID = srchanged.ID Where srchanged.ProcessInstanceID = @ProcessInstanceId
	
	
	Update TOneWhs_BE.SaleEntityService
	Set EED = zschanged.EED
	from TOneWhs_BE.SaleEntityService zs join TOneWhS_BE.CP_SaleZoneServices_Changed zschanged with(nolock)
	on zs.ID = zschanged.ID Where zschanged.ProcessInstanceID = @ProcessInstanceId

	Update TOneWhs_BE.SaleEntityRoutingProduct
	Set EED = rpchanged.EED
	from TOneWhs_BE.SaleEntityRoutingProduct rp join TOneWhS_BE.CP_SaleZoneRoutingProducts_Changed rpchanged with(nolock)
	on rp.ID = rpchanged.ID Where rpchanged.ProcessInstanceID = @ProcessInstanceId

	COMMIT TRAN
	End Try
	
	Begin Catch
	If @@TranCount>0
		RollBack Tran
		
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;
		
		SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
	End Catch 

END