
CREATE PROCEDURE [dbo].[bp_RT_Part_BuildZoneRates]
	 @RebuildZoneRates BIT = 1
	,@CheckToD BIT = 1 
	,@IncludeBlockedZones BIT = 1
AS
BEGIN
		
CREATE TABLE #TempZoneRates(ZoneID INT,SupplierID VARCHAR(5) NULL,CustomerID VARCHAR(5) NULL,[ServicesFlag] [smallint] NULL,ProfileID INT NULL,[ActiveRate] [real] NULL,IsTOD BIT NULL,IsBlock BIT NULL, CodeGroup [VARCHAR](15) NULL)
	SET NOCOUNT ON;
	DECLARE @when datetime
	SET @when = getdate()
	
IF(@RebuildZoneRates = 0)
	BEGIN
				INSERT INTO #SaleZoneRates
	    SELECT zr.ZoneID
			   ,zr.SupplierID
			   ,zr.CustomerID
			   ,zr.ServicesFlag
			   ,zr.ProfileId
			   ,zr.ActiveRate
			   ,zr.IsTOD
			   ,zr.IsBlock
			   ,zr.CodeGroup
	     FROM ZoneRates zr
	     INNER JOIN #SaleZoneFilter szf ON zr.ZoneID = 	szf.ZoneID
	     INNER JOIN #CustomerFilter cf ON zr.CustomerID COLLATE DATABASE_DEFAULT = cf.CustomerID;	
	     
	    INSERT INTO #CostZoneRates
	    SELECT zr.ZoneID
			   ,zr.SupplierID
			   ,zr.CustomerID
			   ,zr.ServicesFlag
			   ,zr.ProfileId
			   ,zr.ActiveRate
			   ,zr.IsTOD
			   ,zr.IsBlock
			   ,zr.CodeGroup
	     FROM ZoneRates zr
	     INNER JOIN #CostZoneFilter czf ON zr.ZoneID = czf.ZoneID	
		 INNER JOIN #ActiveSppliers ac ON zr.SupplierID = ac.CarrierAccountID;
		
		

         
	END
	
ELSE
	BEGIN

	     ;WITH
		 BlocksCTE As(
				SELECT * FROM [fn_RT_Full_GetBlockRules] ( 3 ,-2 ,'')
			)

		,PriceListCTE As( Select * from PriceList p WITH(NOLOCK))

	 ,CurrencyCTE As( Select * from Currency WITH(NOLOCK))
	 ,theTODs(ZoneID,SupplierID,CustomerID,RateType) AS 		
		(
		  SELECT t.ZoneID AS ZoneID,  
				 t.SupplierID AS SupplierID,
				 t.CustomerID AS CustomerID,
				 t.RateType AS RateType
				
		  FROM ToDConsideration t WITH(NOLOCK)
		  WHERE
			  t.IsEffective ='Y'
		  AND t.IsActive = 'Y'
		)
	 
		,CommissionCTE As( 	SELECT * FROM Commission Co WITH(NOLOCK)
				WHERE Co.BeginEffectiveDate <= @when AND (Co.EndEffectiveDate IS NULL OR Co.EndEffectiveDate > @when)
		)

		,ZoneFilters AS 
		(
		SELECT * FROM #SaleZoneFilter szf
		WHERE szf.IsActive = 1
		UNION ALL
		SELECT * FROM #CostZoneFilter czf
		WHERE czf.IsActive = 1	
		)

       ,AllZoneRates AS (
		SELECT			
			Ra.ZoneID,
			Ra.PriceListID,
			Ra.ServicesFlag,
			Rate,
			(Ra.OffPeakRate/ C.LastRate) +
			Case When (SC.Tax2 is null or isnull(Ra.OffPeakRate,0) =0) Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.OffPeakRate/ C.LastRate) * SC.Tax2 / 100.0 End + 
			Case When (Co.SupplierID is null or isnull(Ra.OffPeakRate,0) =0)Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.OffPeakRate/ C.LastRate) * Percentage / 100.0  END),0) End  
			OffPeakRate,

			(Ra.WeekendRate/ C.LastRate) +
			Case When (SC.Tax2 is null or isnull(Ra.WeekendRate,0) =0) Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.WeekendRate/ C.LastRate) * SC.Tax2 / 100.0 End + 
			Case When (Co.SupplierID is null or isnull(Ra.WeekendRate,0) =0) Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.WeekendRate/ C.LastRate) * Percentage / 100.0  END),0) End  
			WeekendRate,
				CASE 
				WHEN @CheckTOD = 1 AND t.RateType = 1 THEN 1 -- OffPeak
				WHEN @CheckTOD = 1 AND t.RateType = 2 THEN 1 -- Weekend 
				WHEN @CheckTOD = 1 AND t.RateType = 4 THEN 1-- Holiday
				ELSE 0 END IsTOD,
			CASE 
				WHEN @CheckTOD = 1 AND t.RateType = 1 THEN ISNULL(OffPeakRate, Ra.Rate) -- OffPeak
				WHEN @CheckTOD = 1 AND t.RateType = 2 THEN ISNULL(WeekendRate,Ra.Rate) -- Weekend 
				WHEN @CheckTOD = 1 AND t.RateType = 4 THEN ISNULL(WeekendRate,Ra.Rate) -- Holiday
				ELSE 
					(Ra.Rate/ C.LastRate) +
			Case When SC.Tax2 is null Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.Rate/ C.LastRate) * SC.Tax2 / 100.0 End + 
			Case When Co.SupplierID is null Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.Rate/ C.LastRate) * Percentage / 100.0  END),0) End  
			END ActiveRate,
			zf.CodeGroup
		FROM Rate Ra WITH(NOLOCK)
			INNER JOIN PriceListCTE  P WITH(NOLOCK) ON Ra.PriceListID = P.PriceListID
			INNER JOIN #ActiveSuppliers SC WITH(NOLOCK) ON P.SupplierID COLLATE DATABASE_DEFAULT = SC.CarrierAccountID
			Left JOIN CurrencyCTE C WITH(NOLOCK) ON P.CurrencyID = C.CurrencyID
			left JOIN CommissionCTE Co WITH(NOLOCK) ON P.SupplierID = Co.SupplierID	AND P.CustomerID = Co.CustomerID AND Ra.ZoneID = Co.ZoneID and Rate between co.FromRate and co.ToRate
			LEFT JOIN theTODs t WITH(NOLOCK) ON Ra.ZoneID = t.ZoneID AND P.CustomerID = t.CustomerID AND P.SupplierID = t.SupplierID
			INNER JOIN ZoneFilters zf ON ra.ZoneID = zf.ZoneID
          WHERE dateadd(MI,isnull(SC.GMTTime,0),Ra.BeginEffectiveDate) <= @when AND (Ra.EndEffectiveDate IS NULL OR dateadd(MI,isnull(SC.GMTTime,0),Ra.EndEffectiveDate) > @when)
   		)
   		
   				 
		INSERT INTO #TempZoneRates
		SELECT			
			Ra.ZoneID,
			P.SupplierID,
			P.CustomerID,			
			Ra.ServicesFlag,
			Case When P.SupplierID ='SYS' Then CS.ProfileID Else SC.ProfileID End ProfileID,
			ActiveRate,
			Ra.IsTOD,
			CASE WHEN @IncludeBlockedZones = 1 AND rb.CustomerID = 'SYS' and P.CustomerID = 'Sys' AND Ra.ZoneID = Rb.ZoneID AND P.SupplierID = Rb.SupplierID THEN 1
				 WHEN @IncludeBlockedZones = 1  AND Rb.CustomerID IS NOT null and P.CustomerID = Rb.CustomerID AND ra.ZoneID = rb.zoneid AND p.supplierid = 'sys' THEN 1
				 ELSE 0
			END 
			 IsBlocked,
			ra.CodeGroup
			
		FROM AllZoneRates Ra WITH(NOLOCK)
				INNER JOIN PriceListCTE  P WITH(NOLOCK) ON Ra.PriceListID = P.PriceListID
				INNER JOIN #CustomerFilter CS WITH(NOLOCK) ON P.CustomerID COLLATE DATABASE_DEFAULT = CS.CustomerID
				INNER JOIN #ActiveSuppliers SC WITH(NOLOCK) ON P.SupplierID COLLATE DATABASE_DEFAULT = SC.CarrierAccountID
				left JOIN BlocksCTE RB WITH(NOLOCK) on  Ra.ZoneID = RB.ZoneID AND P.SupplierID = RB.SupplierID AND P.CustomerID = RB.CustomerID
			WHERE  (
					@IncludeBlockedZones = 1 
					OR
					RB.ZoneID is  null 
			);
		


		INSERT INTO #SaleZoneRates
	    SELECT zr.ZoneID
			   ,zr.SupplierID
			   ,zr.CustomerID
			   ,zr.ServicesFlag
			   ,zr.ProfileId
			   ,zr.ActiveRate
			   ,zr.IsTOD
			   ,zr.IsBlock
			   ,zr.CodeGroup
	     FROM #TempZoneRates zr
	    WHERE zr.SupplierID = 'SYS';
	   
	     
	    INSERT INTO #CostZoneRates
	    SELECT zr.ZoneID
			   ,zr.SupplierID
			   ,zr.CustomerID
			   ,zr.ServicesFlag
			   ,zr.ProfileId
			   ,zr.ActiveRate
			   ,zr.IsTOD
			   ,zr.IsBlock
			   ,zr.CodeGroup
	     FROM #TempZoneRates zr
	    WHERE zr.CustomerID = 'SYS';
	  END


CREATE NONCLUSTERED INDEX [IX_#SaleZoneRates_ServicesFlag] ON #SaleZoneRates([ServicesFlag] ASC)
CREATE NONCLUSTERED INDEX [IX_#SaleZoneRates_SupplierIsBlock] ON #SaleZoneRates(	[SupplierID] ASC,	[IsBlock] ASC, [ZoneID] ASC)
			INCLUDE ( [CustomerID],[ServicesFlag],[ProfileId],[ActiveRate],[IsTOD]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_#CostZoneRates_ServicesFlag] ON #CostZoneRates([ServicesFlag] ASC)
CREATE NONCLUSTERED INDEX [IX_#CostZoneRates_SupplierIsBlock] ON #CostZoneRates(	[SupplierID] ASC,	[IsBlock] ASC, [ZoneID] ASC)
			INCLUDE ( [CustomerID],[ServicesFlag],[ProfileId],[ActiveRate],[IsTOD]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

----Write Back
DELETE zr FROM ZoneRates zr
INNER JOIN #TempZoneRates tzr ON tzr.SupplierID COLLATE DATABASE_DEFAULT = zr.SupplierID AND tzr.CustomerID COLLATE DATABASE_DEFAULT = zr.CustomerID AND tzr.ZoneID = zr.ZoneID

INSERT INTO ZoneRates
SELECT * FROM #TempZoneRates tzr
--Testing	    
SELECT 'SaleZoneRates',* FROM #SaleZoneRates
SELECT 'CostZoneRates',* FROM #CostZoneRates
END