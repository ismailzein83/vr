-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_BillingBridge_GetTariff]
	@When DATETIME,
	@SupplierID VARCHAR(10) = NULL,
	@CustomerID VARCHAR(10) = NULL,
	@CheckChanges CHAR(1) = 'N',
	@TariffTimestamp TIMESTAMP= NULL
AS
BEGIN
IF @CheckChanges = 'Y'
	BEGIN
		;WITH 
            _Zone AS 
            (
                SELECT z.ZoneID,z.Name,z.CodeGroup 
                FROM Zone Z WITH(NOLOCK)
                WHERE  (Z.IsEffective = 'Y' or z.BeginEffectiveDate>@When)
                and Z.SupplierID=@SupplierID
            )
            ,_Codez AS
             (
                SELECT C.ZoneID,C.Code,C.BeginEffectiveDate,C.EndEffectiveDate
                FROM Code C WITH(NOLOCK)
                WHERE (C.IsEffective = 'Y' or c.BeginEffectiveDate>@When)AND 
                c.ZoneID IN (SELECT zoneid FROM _Zone)
             )
             ,_Ratez AS
             (
					SELECT R.rateID,R.ZoneID,R.BeginEffectiveDate,R.EndEffectiveDate,R.Rate,z.Name,z.CodeGroup FROM Rate R 
					JOIN _Zone z ON z.ZoneID = R.ZoneID
					WHERE R.PriceListID IN (SELECT PriceListID FROM PriceList WHERE CustomerID = @CustomerID AND SupplierID = @SupplierID)
                    --AND  R.IsEffective='Y'     
             )
             SELECT T.ZoneID
					,R.Name
					,R.CodeGroup
					,T.FirstPeriodRate
					,T.FirstPeriod
					,T.FractionUnit
					,C.Code
					,C.BeginEffectiveDate codebegineffectivedate
					,C.EndEffectiveDate codeEndEffectiveDate
					,R.BeginEffectiveDate
					,R.EndEffectiveDate
					,R.RateID
					,R.Rate
					,T.BeginEffectiveDate AS TBED
					,T.EndEffectiveDate AS TEED
					,t.iseffective,t.TariffID
             FROM Tariff T
             JOIN _Codez C ON C.ZoneID = T.ZoneID
             JOIN _Ratez R ON R.ZoneID= T.ZoneID
             WHERE T.CustomerID=@CustomerID  AND T.[timestamp] > @TariffTimestamp AND T.SupplierID=@SupplierID
END
ELSE
	BEGIN
		;WITH 
		EffZone AS 
		(
			SELECT z.ZoneID,z.Name,z.CodeGroup 
			FROM Zone Z WITH(NOLOCK)
			WHERE Z.SupplierID=@SupplierID AND Z.IsEffective='Y'
		)
		,_Zone AS 
		(
			SELECT z.ZoneID,z.Name,z.CodeGroup 
			FROM Zone Z WITH(NOLOCK)
			WHERE Z.SupplierID=@SupplierID 
		)
		,EffCodez AS
		 (
			SELECT C.ZoneID,C.Code,C.BeginEffectiveDate,C.EndEffectiveDate
			FROM Code C WITH(NOLOCK)
			WHERE C.IsEffective='Y'
			AND C.ZoneID IN ( SELECT ZoneID FROM EffZone)

		 )
		 ,Codez AS
		 (
			SELECT C.ZoneID,C.Code,C.BeginEffectiveDate,C.EndEffectiveDate
			FROM Code C WITH(NOLOCK)
			WHERE C.ZoneID IN ( SELECT ZoneID FROM _Zone )
		--	and c.endeffectivedate> @when

		 )
		 ,EffRatez AS
		 (
				SELECT R.ZoneID,R.BeginEffectiveDate,R.EndEffectiveDate,R.Rate,R.RateID
				FROM Rate R 
				WHERE R.IsEffective='Y'
				AND R.PriceListID IN (SELECT PriceListID FROM PriceList WHERE CustomerID=@CustomerID AND SupplierID=@SupplierID)
		 )
		 ,Ratez AS
		 (
				SELECT R.ZoneID,R.BeginEffectiveDate,R.EndEffectiveDate,R.Rate,R.RateID
				FROM Rate R 
				WHERE R.EndEffectiveDate >=@When AND ( R.EndEffectiveDate IS null or R.EndEffectiveDate <> R.BeginEffectiveDate)
				AND R.PriceListID IN (SELECT PriceListID FROM PriceList WHERE CustomerID=@CustomerID AND SupplierID = @SupplierID)
		 )
		 ,EffResult
		 AS 
		 (
			SELECT T.ZoneID,CZ.Name,T.FirstPeriodRate,T.FirstPeriod,T.FractionUnit,C.Code,CZ.CodeGroup,C.BeginEffectiveDate AS codebegineffectivedate,C.EndEffectiveDate AS codeEndEffectiveDate,
					R.BeginEffectiveDate,R.EndEffectiveDate,R.Rate,T.BeginEffectiveDate AS TBED, T.EndEffectiveDate AS TEED,t.iseffective,R.RateID,t.TariffID
			 FROM Tariff T
			 JOIN EffZone CZ ON CZ.ZoneID = T.ZoneID
			 JOIN EffCodez C ON C.ZoneID = T.ZoneID
			 JOIN EffRatez R ON R.ZoneID= T.ZoneID
			 WHERE T.IsEffective='Y' AND T.CustomerID=@CustomerID AND T.SupplierID=@SupplierID
		 )
		, _AllResult 
		 AS
		 (
			SELECT T.ZoneID,CZ.Name,T.FirstPeriodRate,T.FirstPeriod,T.FractionUnit,C.Code,CZ.CodeGroup,C.BeginEffectiveDate AS codebegineffectivedate,C.EndEffectiveDate AS codeEndEffectiveDate,
				   R.BeginEffectiveDate,R.EndEffectiveDate,R.Rate,T.BeginEffectiveDate AS TBED, T.EndEffectiveDate AS TEED,t.iseffective,R.RateID,t.TariffID
			FROM Tariff T
			 JOIN _Zone CZ ON CZ.ZoneID = T.ZoneID
			 JOIN Codez C ON C.ZoneID = T.ZoneID
			 JOIN Ratez R ON R.ZoneID= T.ZoneID
			 WHERE ( T.EndEffectiveDate>=@When )
			 AND  T.CustomerID=@CustomerID AND T.SupplierID=@SupplierID
			 AND r.EndEffectiveDate > t.BeginEffectiveDate
		 )
		 ,FullResult 
		 AS ( SELECT * FROM EffResult UNION select * from _AllResult)

		 SELECT * FROM FullResult
	END
END