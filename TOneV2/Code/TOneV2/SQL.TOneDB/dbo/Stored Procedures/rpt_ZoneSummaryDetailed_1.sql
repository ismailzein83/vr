



CREATE PROCEDURE [dbo].[rpt_ZoneSummaryDetailed](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Cost char(1)='Y',
	@CurrencyID varchar(3),
	@SupplierGroup VARCHAR(max)=NULL,
	@CustomerGroup VARCHAR(max)=NULL,
	@CustomerAmuID int = NULL,
	@SupplierAmuID int = NULL,
	@GroupBySupplier char(1) = 'N'
)

	AS 

DECLARE @CustomerIDs TABLE( CarrierAccountID VARCHAR(5) )
DECLARE @SupplierIDs TABLE( CarrierAccountID VARCHAR(5) )


IF(@CustomerAMUID IS NOT NULL)
	BEGIN
		DECLARE @customerAmuFlag VARCHAR(20)
		SET @customerAmuFlag = (SELECT Flag FROM AMU WHERE ID = @CustomerAMUID)
		INSERT INTO @CustomerIDs
		SELECT ac.CarrierAccountID
		FROM AMU_Carrier ac
		WHERE ac.AMUCarrierType = 0
		AND ac.AMUID IN (
			SELECT ID FROM AMU
			WHERE Flag LIKE @customerAmuFlag + '%'
			)
	END

	IF(@SupplierAMUID IS NOT NULL)
	BEGIN	
		DECLARE @supplierAmuFlag VARCHAR(20)
		SET @supplierAmuFlag = (SELECT Flag FROM AMU WHERE ID = @SupplierAMUID)
		INSERT INTO @SupplierIDs
		SELECT ac.CarrierAccountID
		FROM AMU_Carrier ac
		WHERE ac.AMUCarrierType = 1
		AND ac.AMUID IN (
			SELECT ID FROM AMU
			WHERE Flag LIKE @supplierAmuFlag + '%'
			)
	END

DECLARE @ExchangeRates TABLE(
		CurrencyIn VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
		CurrencyOut VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(CurrencyIn,CurrencyOut, Date))

INSERT INTO @ExchangeRates 
SELECT gder1.Currency AS CurrencyIn,
		   gder2.Currency AS CurrencyOut,
		   gder1.Date AS Date, 
		   gder1.Rate / gder2.Rate AS Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1
JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2 ON  gder1.Date = gder2.Date


IF(@GroupBySupplier = 'N')
BEGIN
	IF (@Cost = 'Y')
	BEGIN 
		DECLARE @NumberOfDays INT 
		SET @NumberOfDays = DATEDIFF(dd,@FromDate,@ToDate) + 1
		IF (@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT
				Z.Name AS Zone,
				Z.ZoneID as zoneID,
				SUM(bs.NumberOfCalls) AS Calls,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
				ISNULL(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS Rate,
				--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
				
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
				ISNULL(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS OffPeakRate,
				--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
				
				SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.CostDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
				ISNULL(AVG(CASE WHEN bs.Cost_RateType =2 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) END),0) AS WeekEndRate,	
				SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
				
				SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
				SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
			AND(@SupplierGroup IS  NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			AND(@CustomerGroup IS NULL  OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' )
			GROUP BY Z.ZoneID,z.Name,bs.SupplierID
			ORDER BY Z.Name ASC 
		END
		ELSE
			IF (@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN 
				SELECT
					Z.Name AS Zone,
					Z.ZoneID as zoneID,
					SUM(bs.NumberOfCalls) AS Calls,
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
					SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
					--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
					ISNULL(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS Rate,
					--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
					SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
					
					SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
					--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
					ISNULL(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS OffPeakRate,
					--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
					SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
					
					SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.CostDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
					ISNULL(AVG(CASE WHEN bs.Cost_RateType =2 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS WeekEndRate,	
					SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
				
					SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
					SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
					SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID   
				LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
				WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
					 AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
					 AND bs.SupplierID=@SupplierID
					 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
					 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
				GROUP BY z.ZoneID,Z.Name,bs.SupplierID
				ORDER BY Z.Name ASC
			END
			ELSE 
				IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
				BEGIN 
					SELECT
						Z.Name AS Zone,
						Z.ZoneID as zoneID,
						SUM(bs.NumberOfCalls) AS Calls,
						SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
						SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
						--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
						ISNULL(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS Rate,
						--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
						SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
						
						SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
						--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
						ISNULL(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) END),0) AS OffPeakRate,
						--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
						SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
						
						SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.CostDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
						ISNULL(AVG(CASE WHEN bs.Cost_RateType =2 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS WeekEndRate,	
						SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  WeekEndNet,

						SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
						SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
						SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID   
					JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = BS.CallDate
					WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
						 AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
						 AND bs.CustomerID=@CustomerID
						 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
						 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
					GROUP BY Z.Name,Z.ZoneID,bs.SupplierID
					ORDER BY Z.Name ASC 
				END
				ELSE
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
					BEGIN 
						SELECT
							Z.Name AS Zone,
							Z.ZoneID as zoneID,
							SUM(bs.NumberOfCalls) AS Calls,
							SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
							SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
							--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
							ISNULL(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) END),0) AS Rate,
							--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
							SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
							
							SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
							--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
							ISNULL(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS OffPeakRate,
							--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
							SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
							
							SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.CostDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
							ISNULL(AVG(CASE WHEN bs.Cost_RateType =2 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS WeekEndRate,	
							SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
							
							SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
							SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
							SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
						LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID   
						LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
						WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
							 AND bs.CustomerID=@CustomerID 
							 AND bs.SupplierID=@SupplierID
							 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
							 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
						GROUP BY Z.Name,Z.ZoneID,bs.SupplierID
						ORDER BY Z.Name ASC 
					END
--					SELECT isnull(SUM(ca.Services + ca.ConnectionFees) * @NumberOfDays,0) AS Services
--					FROM CarrierAccount ca 
--					WHERE ca.CarrierAccountID IN 
--     					(
--						SELECT DISTINCT bs.SupplierID	
--						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
--						WHERE  bs.CallDate >= @FromDate	AND  bs.CallDate <= @ToDate
--						)

				SELECT ((SUM((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1)))*@NumberOfDays) AS Services
				FROM CarrierAccount ca 
				JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
				LEFT JOIN @ExchangeRates ER ON ER.CurrencyIn = cp.CurrencyID AND ER.CurrencyOut = @CurrencyID AND ER.Date = @FromDate
				WHERE ca.CarrierAccountID = @CustomerID OR @CustomerID IS NULL
				
	END 
	ELSE 
	BEGIN 
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
			BEGIN 
				SELECT 
					Z.Name AS Zone,
					Z.ZoneID as zoneID,
					bs.CustomerID,
					SUM(bs.NumberOfCalls) AS Calls,
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
					SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
					--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
					ISNULL(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) END),0) AS Rate,
					--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
					SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
					
					SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
					--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
					ISNULL(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1))  END),0) AS OffPeakRate,
					--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
					SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
					
					SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
					ISNULL(AVG(CASE WHEN bs.Sale_RateType =2 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) END),0) AS WeekEndRate,	
					SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.Sale_Nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  WeekEndNet,

					SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
					SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
					SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
				LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
				WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
				AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
				AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
                AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			    AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
				GROUP BY Z.Name,Z.ZoneID,bs.CustomerID
				ORDER BY Z.Name ASC
			END
			ELSE
				IF(@CustomerID IS NULL AND @SupplierID IS NOT NULL)
				BEGIN 
					SELECT 
						Z.Name AS Zone,
						Z.ZoneID as zoneID,
						bs.CustomerID,
						SUM(bs.NumberOfCalls) AS Calls,
						SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
						SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
						--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
						ISNULL(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS Rate,
						--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
						SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
					
						SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
						--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
						ISNULL(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1))  END),0) AS OffPeakRate,
						--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
						SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
											
						SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
						ISNULL(AVG(CASE WHEN bs.Sale_RateType =2 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS WeekEndRate,	
						SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.Sale_Nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
						
						SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
						SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
						SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
					LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
					WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
						 AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
						 AND bs.SupplierID=@SupplierID
						 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
						 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
					GROUP BY Z.Name,Z.ZoneID,bs.CustomerID
					ORDER BY Z.Name ASC
				END
				ELSE 
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
					BEGIN 
						SELECT 
								Z.Name AS Zone,
								Z.ZoneID as zoneID,
								SUM(bs.NumberOfCalls) AS Calls,
								SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS Rate,
								--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
								
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) END),0) AS OffPeakRate,
								--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
								
								SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType =2 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS WeekEndRate,	
								SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.Sale_Nets/ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
						
								SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
								SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
								SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
							FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
							LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
							LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
							WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
								 AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
								 AND bs.CustomerID=@CustomerID
						         AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
								 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
							GROUP BY Z.Name,Z.ZoneID,bs.CustomerID
							ORDER BY Z.Name ASC
						
					END
					ELSE
						IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
						BEGIN 
							SELECT 
								Z.Name AS Zone,
								Z.ZoneID as zoneID,
								SUM(bs.NumberOfCalls) AS Calls,
								SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) END),0) AS Rate,
								--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
								
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1))  END),0) AS OffPeakRate,
								--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
								
								SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType =2 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS WeekEndRate,	
								SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.Sale_Nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
						
								SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
								SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
								SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
							FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
							LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
							LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
							WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
								 AND bs.CustomerID=@CustomerID
								 AND bs.SupplierID=@SupplierID
								 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
								 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 					
							GROUP BY Z.Name,Z.ZoneID,bs.CustomerID
							ORDER BY Z.Name ASC
						END	
    END 
END
ELSE
BEGIN
		IF (@Cost = 'Y')
	BEGIN 
		DECLARE @NumberOfDays1 INT 
		SET @NumberOfDays1 = DATEDIFF(dd,@FromDate,@ToDate) + 1
		IF (@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT
				Z.Name AS Zone,
				Z.ZoneID as zoneID,
				bs.SupplierID as SupplierID,
				SUM(bs.NumberOfCalls) AS Calls,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
				ISNULL(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS Rate,
				--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
				
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
				ISNULL(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS OffPeakRate,
				--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
				
				SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.CostDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
				ISNULL(AVG(CASE WHEN bs.Cost_RateType =2 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) END),0) AS WeekEndRate,	
				SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
				
				SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
				SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
			AND(@SupplierGroup IS  NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			AND(@CustomerGroup IS NULL  OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' )
			GROUP BY Z.ZoneID,z.Name,bs.SupplierID
			ORDER BY Z.Name ASC 
		END
		ELSE
			IF (@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN 
				SELECT
					Z.Name AS Zone,
					Z.ZoneID as zoneID,
					bs.SupplierID as SupplierID,
					SUM(bs.NumberOfCalls) AS Calls,
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
					SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
					--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
					ISNULL(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS Rate,
					--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
					SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
					
					SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
					--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
					ISNULL(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS OffPeakRate,
					--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
					SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
					
					SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.CostDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
					ISNULL(AVG(CASE WHEN bs.Cost_RateType =2 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS WeekEndRate,	
					SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
				
					SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
					SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
					SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
				LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
				WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
					 AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
					 AND bs.SupplierID=@SupplierID
					 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
					 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
				GROUP BY z.ZoneID,Z.Name,bs.SupplierID
				ORDER BY Z.Name ASC
			END
			ELSE 
				IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
				BEGIN 
					SELECT
						Z.Name AS Zone,
						Z.ZoneID as zoneID,
						bs.SupplierID as SupplierID,
						SUM(bs.NumberOfCalls) AS Calls,
						SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
						SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
						--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
						ISNULL(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS Rate,
						--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
						SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
						
						SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
						--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
						ISNULL(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) END),0) AS OffPeakRate,
						--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
						SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
						
						SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.CostDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
						ISNULL(AVG(CASE WHEN bs.Cost_RateType =2 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS WeekEndRate,	
						SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  WeekEndNet,

						SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
						SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
						SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
					JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = BS.CallDate
					WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
						 AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
						 AND bs.CustomerID=@CustomerID
						 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
						 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
					GROUP BY Z.Name,Z.ZoneID,bs.SupplierID
					ORDER BY Z.Name ASC 
				END
				ELSE
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
					BEGIN 
						SELECT
							Z.Name AS Zone,
							Z.ZoneID as zoneID,
							bs.SupplierID as SupplierID,
							SUM(bs.NumberOfCalls) AS Calls,
							SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
							SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
							--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
							ISNULL(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) END),0) AS Rate,
							--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
							SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
							
							SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
							--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
							ISNULL(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS OffPeakRate,
							--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
							SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
							
							SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.CostDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
							ISNULL(AVG(CASE WHEN bs.Cost_RateType =2 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1))  END),0) AS WeekEndRate,	
							SUM(CASE WHEN bs.Cost_RateType = 2 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
							
							SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
							SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
							SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
						LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
						LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
						WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
							 AND bs.CustomerID=@CustomerID 
							 AND bs.SupplierID=@SupplierID
							 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
							 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
						GROUP BY Z.Name,Z.ZoneID,bs.SupplierID
						ORDER BY Z.Name ASC 
					END
--					SELECT isnull(SUM(ca.Services + ca.ConnectionFees) * @NumberOfDays,0) AS Services
--					FROM CarrierAccount ca 
--					WHERE ca.CarrierAccountID IN 
--     					(
--						SELECT DISTINCT bs.SupplierID	
--						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
--						WHERE  bs.CallDate >= @FromDate	AND  bs.CallDate <= @ToDate
--						)

				SELECT ((SUM((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1)))*@NumberOfDays1) AS Services
				FROM CarrierAccount ca 
				JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
				LEFT JOIN @ExchangeRates ER ON ER.CurrencyIn = cp.CurrencyID AND ER.CurrencyOut = @CurrencyID AND ER.Date = @FromDate
				WHERE ca.CarrierAccountID = @CustomerID OR @CustomerID IS NULL
				
	END 
	ELSE 
	BEGIN 
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
			BEGIN 
				SELECT 
					Z.Name AS Zone,
					Z.ZoneID as zoneID,
					bs.SupplierID as SupplierID,
					bs.CustomerID,
					SUM(bs.NumberOfCalls) AS Calls,
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
					SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
					--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
					ISNULL(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) END),0) AS Rate,
					--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
					SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
					
					SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
					--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
					ISNULL(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1))  END),0) AS OffPeakRate,
					--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
					SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
					
					SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
					ISNULL(AVG(CASE WHEN bs.Sale_RateType =2 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) END),0) AS WeekEndRate,	
					SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.Sale_Nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  WeekEndNet,

					SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
					SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
					SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
				LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
				WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
				AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
				AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
                AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			    AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
				GROUP BY Z.Name,Z.ZoneID,bs.CustomerID, bs.SupplierID
				ORDER BY Z.Name ASC
			END
			ELSE
				IF(@CustomerID IS NULL AND @SupplierID IS NOT NULL)
				BEGIN 
					SELECT 
						Z.Name AS Zone,
						Z.ZoneID as zoneID,
						bs.CustomerID,
						bs.SupplierID as SupplierID,
						SUM(bs.NumberOfCalls) AS Calls,
						SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
						SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
						--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
						ISNULL(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS Rate,
						--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
						SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
					
						SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
						--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
						ISNULL(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1))  END),0) AS OffPeakRate,
						--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
						SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
											
						SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
						ISNULL(AVG(CASE WHEN bs.Sale_RateType =2 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS WeekEndRate,	
						SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.Sale_Nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
						
						SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
						SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
						SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
					LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
					WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
						 AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
						 AND bs.SupplierID=@SupplierID
						 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
						 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
					GROUP BY Z.Name,Z.ZoneID,bs.CustomerID, bs.SupplierID
					ORDER BY Z.Name ASC
				END
				ELSE 
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
					BEGIN 
						SELECT 
								Z.Name AS Zone,
								Z.ZoneID as zoneID,
								bs.SupplierID as SupplierID,
								SUM(bs.NumberOfCalls) AS Calls,
								SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS Rate,
								--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
								
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) END),0) AS OffPeakRate,
								--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
								
								SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType =2 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS WeekEndRate,	
								SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.Sale_Nets/ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
						
								SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
								SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
								SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
							FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
							LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
							LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
							WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
								 AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
								 AND bs.CustomerID=@CustomerID
						         AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
								 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
							GROUP BY Z.Name,Z.ZoneID,bs.CustomerID, bs.SupplierID
							ORDER BY Z.Name ASC
						
					END
					ELSE
						IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
						BEGIN 
							SELECT 
								Z.Name AS Zone,
								Z.ZoneID as zoneID,
								bs.SupplierID as SupplierID,
								SUM(bs.NumberOfCalls) AS Calls,
								SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) END),0) AS Rate,
								--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
								
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1))  END),0) AS OffPeakRate,
								--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
								
								SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS WeekEndDurationInSeconds,
								ISNULL(AVG(CASE WHEN bs.Sale_RateType =2 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1))  END),0) AS WeekEndRate,	
								SUM(CASE WHEN bs.Sale_RateType = 2 THEN (bs.Sale_Nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  WeekEndNet,
						
								SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
								SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
								SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
							FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
							LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
							LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
							WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
								 AND bs.CustomerID=@CustomerID
								 AND bs.SupplierID=@SupplierID
								 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
								 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 					
							GROUP BY Z.Name,Z.ZoneID,bs.CustomerID, bs.SupplierID
							ORDER BY Z.Name ASC
						END	
    END 
END
	RETURN