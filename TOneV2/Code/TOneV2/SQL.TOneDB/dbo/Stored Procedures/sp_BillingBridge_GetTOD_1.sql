-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_BillingBridge_GetTOD]
	@When DATETIME,
	@SupplierID VARCHAR(10) = NULL,
	@CustomerID VARCHAR(10) = NULL,
	@CheckChanges CHAR(1)= 'N',
	@TODTimeStamp TIMESTAMP= NULL,
	@RateTimeStamp TIMESTAMP= NULL
AS
BEGIN

IF @CheckChanges ='Y'
	BEGIN
		  ;WITH 
			   _Zonez AS
			   (
				SELECT z.ZoneID,z.Name FROM zone z WHERE z.SupplierID= @SupplierID
				AND ( z.IsEffective='Y' or z.BeginEffectiveDate>@When)
			   )
               ,_PriceListz AS
                (
                    SELECT P.PriceListID 
                    FROM PriceList P WITH(NOLOCK)
                    WHERE P.CustomerID = @CustomerID AND P.SupplierID=@SupplierID
                ) 
                ,_RateChangeOnTOD as
				(
					SELECT R.OffPeakRate,R.WeekendRate,R.Rate,R.ZoneID,r.EndEffectiveDate,Z.Name
					FROM rate R 
					LEFT JOIN _Zonez z ON z.ZoneID = r.ZoneID
					where r.timestamp > @RateTimeStamp
					AND r.PriceListID IN ( SELECT PriceListID FROM pricelist where customerid =@CustomerID)
					AND r.ZoneId in ( select zoneid from todconsideration tod where tod.customerid=@CustomerID AND TOD.SupplierID= @SupplierID and ( tod.iseffective='Y' or tod.begineffectivedate>@When ))
				)
				,_RateChangeResult AS
				(
					SELECT TOD.ToDConsiderationID,Z.Name,TOD.ZoneID,TOD.BeginTime,TOD.EndTime,TOD.HolidayDate,TOD.RateType,TOD.[WeekDay],TOD.BeginEffectiveDate AS TODBED, TOD.EndEffectiveDate AS TODEED,tod.iseffective
					FROM todconsideration tod WITH(NOLOCK)
					LEFT JOIN _Zonez z ON z.ZoneID = tod.ZoneID
					WHERE TOD.CustomerID= @CustomerID AND TOD.SupplierID= @SupplierID  and ( tod.iseffective='Y' or tod.begineffectivedate>@When )
					 and tod.zoneid in (select zoneid from _RateChangeOnTOD)
				)
                ,_Ratez AS
                (
					SELECT R.OffPeakRate,R.WeekendRate,R.Rate,R.ZoneID,r.EndEffectiveDate
					FROM dbo.Rate R
					WHERE --R.IsEffective='Y' AND 
					R.PriceListID IN ( SELECT PriceListID FROM _PriceListz)
                )
                SELECT TOD.ToDConsiderationID,Z.Name,TOD.ZoneID,TOD.BeginTime,TOD.EndTime,TOD.HolidayDate,TOD.RateType,TOD.[WeekDay],TOD.BeginEffectiveDate AS TODBED, TOD.EndEffectiveDate AS TODEED,tod.iseffective
                FROM dbo.ToDConsideration TOD WITH(NOLOCK)                
				LEFT JOIN _Zonez z ON z.ZoneID = tod.ZoneID
                WHERE TOD.CustomerID= @CustomerID AND TOD.SupplierID= @SupplierID  AND TOD.[timestamp] > @TODTimeStamp 
                and tod.zoneid in (select zoneid from _Ratez)
                union (Select * from _RateChangeResult)
   	END
ELSE
	BEGIN
		;WITH 
		EffZonez AS
		(
			SELECT z.ZoneID,z.Name,z.CodeGroup 
			FROM Zone Z WITH(NOLOCK)
			WHERE Z.SupplierID=@SupplierID AND Z.IsEffective='Y'
		)
		,Zonez AS
		(
			SELECT z.ZoneID,z.Name,z.CodeGroup 
			FROM Zone Z WITH(NOLOCK)
			WHERE (z.EndEffectiveDate IS NULL OR z.EndEffectiveDate >=@When )
			AND Z.SupplierID=@SupplierID
		)
		,EffCustPriceListz AS
		(
			SELECT P.PriceListID 
			FROM PriceList P WITH(NOLOCK)
			WHERE P.CustomerID = @CustomerID AND P.SupplierID=@SupplierID AND p.IsEffective ='Y'
		)
		,PriceListz AS
		(
			SELECT P.PriceListID 
			FROM PriceList P WITH(NOLOCK)
			WHERE P.CustomerID = @CustomerID AND P.SupplierID=@SupplierID 
		)
		,_Ratez AS
		(
			SELECT  R.OffPeakRate,R.WeekendRate,R.Rate,R.ZoneID,Z.Name,r.EndEffectiveDate
			FROM RATE R  WITH(NOLOCK)
			JOIN Zonez Z ON Z.ZoneID = R.ZoneID
			WHERE R.PriceListID IN ( SELECT PriceListID FROM PriceListz)
		)
		,_EffRatez AS
		(
			SELECT  R.OffPeakRate,R.WeekendRate,R.Rate,R.ZoneID,Z.Name,r.EndEffectiveDate
			FROM RATE R  WITH(NOLOCK)
			JOIN EffZonez Z ON Z.ZoneID = R.ZoneID
			WHERE R.PriceListID IN ( SELECT PriceListID FROM EffCustPriceListz)
			AND R.IsEffective ='Y'
		)
		,_EffResultz AS 
		(
			SELECT TOD.ToDConsiderationID,R.Name,R.ZoneID,TOD.BeginTime,TOD.EndTime,TOD.HolidayDate,TOD.RateType,TOD.[WeekDay],TOD.BeginEffectiveDate AS TODBED, TOD.EndEffectiveDate AS TODEED,TOD.IsEffective
			FROM ToDConsideration TOD WITH(NOLOCK)
			JOIN EffZonez R ON R.ZoneID = TOD.ZoneID
			WHERE TOD.CustomerID=@CustomerID AND TOD.IsEffective='Y' AND TOD.[WeekDay] IS NOT NULL AND TOD.SupplierID=@supplierid
			and tod.zoneid in ( select zoneid from _EffRatez)
		)
		,_Resultz AS 
		(	
			SELECT TOD.ToDConsiderationID,R.Name,R.ZoneID,TOD.BeginTime,TOD.EndTime,TOD.HolidayDate,TOD.RateType,TOD.[WeekDay],TOD.BeginEffectiveDate AS TODBED, TOD.EndEffectiveDate AS TODEED,TOD.IsEffective
			FROM ToDConsideration TOD WITH(NOLOCK)
			JOIN Zonez R ON R.ZoneID = TOD.ZoneID
			WHERE (tod.EndEffectiveDate IS NULL OR  TOD.EndEffectiveDate >=@When )
			AND TOD.CustomerID=@CustomerID AND TOD.[WeekDay] IS NOT NULL AND TOD.SupplierID=@supplierid
			-- AND r.EndEffectiveDate > tod.BeginEffectiveDate
			 and tod.zoneid in ( select zoneid from _Ratez)
			
		)
		,_Finalz AS 
		( SELECT * FROM _EffResultz UNION SELECT * FROM _Resultz)
		SELECT * FROM _Finalz
	END
END