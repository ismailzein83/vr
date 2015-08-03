


CREATE  PROCEDURE [dbo].[rpt_ZoneSummary](
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

IF(@GroupBySupplier = 'N')
BEGIN
	IF (@Cost = 'Y')
	BEGIN 
		DECLARE @NumberOfDays INT 
		SET @NumberOfDays = DATEDIFF(dd,@FromDate,@ToDate) + 1
		
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
				AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
				--bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
				bs.Cost_RateType AS RateType,
				SUM(bs.CostDuration/60.0) AS DurationInSeconds,
				SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
				SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
				AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
				AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
				AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			    AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
			GROUP BY Z.Name,bs.Cost_RateType--,bs.Cost_Rate/ISNULL(ERC.Rate, 1),bs.Cost_RateType
			ORDER BY Z.Name ASC 
		END
		ELSE
			IF (@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN 
			SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
				AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
				--bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
				bs.Cost_RateType AS RateType,
				SUM(bs.CostDuration/60.0) AS DurationInSeconds,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
				SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
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
			GROUP BY Z.Name,bs.Cost_RateType--,bs.Cost_Rate/ISNULL(ERC.Rate, 1)
			ORDER BY Z.Name ASC 	
			END 
		ELSE 
		    IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
		    BEGIN 
				SELECT 
					Z.Name AS Zone,
					SUM(bs.NumberOfCalls) AS Calls,
					--bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
					AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
					bs.Cost_RateType AS RateType,
					SUM(bs.CostDuration/60.0) AS DurationInSeconds,
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
					SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
					SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
					SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID   
				LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
				WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			         AND(@SupplierAmuID IS NULL OR  bs.SupplierID IN (SELECT * FROM @SupplierIDs))
					 AND bs.CustomerID=@CustomerID
					 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			         AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
				GROUP BY Z.Name,bs.Cost_RateType--,bs.Cost_Rate/ISNULL(ERC.Rate, 1)
				ORDER BY Z.Name ASC
		END
		ELSE
			IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
			BEGIN 
				SELECT 
					Z.Name AS Zone,
					SUM(bs.NumberOfCalls) AS Calls,
					AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
					--bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
					bs.Cost_RateType AS RateType,
					SUM(bs.CostDuration/60.0) AS DurationInSeconds,
					--(bs.Cost_Rate/ISNULL(ERC.Rate, 1))*(SUM(bs.CostDuration/60.0)) AS Net
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
					SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
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
				GROUP BY Z.Name,bs.Cost_RateType--,bs.Cost_Rate/ISNULL(ERC.Rate, 1)
				ORDER BY Z.Name ASC 		
			END

			SELECT ((SUM((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1)))*@NumberOfDays) AS Services
			FROM CarrierAccount ca 
			JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
			LEFT JOIN @ExchangeRates ER ON ER.CurrencyIn = cp.CurrencyID AND ER.CurrencyOut = @CurrencyID AND ER.Date = @FromDate
	END
	ELSE 
	BEGIN 
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
				--bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
				AVG(ISNULL(bs.Sale_Rate/ISNULL(ERS.Rate, 1),0)) AS Rate,
				bs.Sale_RateType AS RateType,
				SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
				--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
				SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
				SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
				SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
			LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
			WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
			AND(@CustomerAmuID IS NULL OR  bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR  bs.SupplierID IN (SELECT * FROM @SupplierIDs))
			AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
		    AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
			GROUP BY Z.Name,bs.Sale_RateType--,bs.Sale_Rate/ISNULL(ERS.Rate, 1)
			ORDER BY Z.Name ASC
		END
		ELSE 
			IF(@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN
				SELECT 
					Z.Name AS Zone,
					SUM(bs.NumberOfCalls) AS Calls,
					AVG(ISNULL(bs.Sale_Rate/ISNULL(ERS.Rate, 1),0)) AS Rate,
					--bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
					bs.Sale_RateType AS RateType,
					SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
					--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
					SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
					SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
					SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
				LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
				WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
					AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
					 AND bs.SupplierID=@SupplierID
					 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			         AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
				GROUP BY Z.Name,bs.Sale_RateType--,bs.Sale_Rate/ISNULL(ERS.Rate, 1)
				ORDER BY Z.Name ASC
			END
			ELSE
				IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
				BEGIN
					SELECT 
						Z.Name AS Zone,
						SUM(bs.NumberOfCalls) AS Calls,
						AVG(ISNULL(bs.Sale_Rate/ISNULL(ERS.Rate, 1),0)) AS Rate,
						--bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
						bs.Sale_RateType AS RateType,
						SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
						--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
						SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
						SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
						SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
						SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
					LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
					WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
						 AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
						 AND bs.CustomerID=@CustomerID
						 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			             AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
					GROUP BY Z.Name,bs.Sale_RateType--,bs.Sale_Rate/ISNULL(ERS.Rate, 1)
					ORDER BY Z.Name ASC
				END
				ELSE
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
					BEGIN
						 SELECT 
							Z.Name AS Zone,
							SUM(bs.NumberOfCalls) AS Calls,
							AVG(ISNULL(bs.Sale_Rate/ISNULL(ERS.Rate, 1),0)) AS Rate,
							--bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
							bs.Sale_RateType AS RateType,
							SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
							--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
							SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
							SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
							SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
							SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
						LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
						LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
						WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
							 AND bs.CustomerID=@CustomerID
							 AND bs.SupplierID=@SupplierID
							 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			                 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
						GROUP BY Z.Name,bs.Sale_RateType--,bs.Sale_Rate/ISNULL(ERS.Rate, 1)
						ORDER BY Z.Name ASC
					END
			END 
    END
ELSE
  BEGIN
    IF (@Cost = 'Y')
	BEGIN 
		DECLARE @NumberOfDays2 INT 
		SET @NumberOfDays2 = DATEDIFF(dd,@FromDate,@ToDate) + 1
		
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT 
				Z.Name AS Zone,
				bs.SupplierID AS SupplierID,
				SUM(bs.NumberOfCalls) AS Calls,
				AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
				--bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
				bs.Cost_RateType AS RateType,
				SUM(bs.CostDuration/60.0) AS DurationInSeconds,
				SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
				SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
				AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
				AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
				AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			    AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
			GROUP BY Z.Name, bs.SupplierID,bs.Cost_RateType--,bs.Cost_Rate/ISNULL(ERC.Rate, 1),bs.Cost_RateType
			ORDER BY Z.Name ASC 
		END
		ELSE
			IF (@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN 
			SELECT 
				Z.Name AS Zone,
				bs.SupplierID AS SupplierID,
				SUM(bs.NumberOfCalls) AS Calls,
				AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
				--bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
				bs.Cost_RateType AS RateType,
				SUM(bs.CostDuration/60.0) AS DurationInSeconds,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
				SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
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
			GROUP BY Z.Name, bs.SupplierID,bs.Cost_RateType--,bs.Cost_Rate/ISNULL(ERC.Rate, 1)
			ORDER BY Z.Name ASC 	
			END 
		ELSE 
		    IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
		    BEGIN 
				SELECT 
					Z.Name AS Zone,
					bs.SupplierID AS SupplierID,
					SUM(bs.NumberOfCalls) AS Calls,
					--bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
					AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
					bs.Cost_RateType AS RateType,
					SUM(bs.CostDuration/60.0) AS DurationInSeconds,
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
					SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
					SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
					SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
				LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
				WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			         AND(@SupplierAmuID IS NULL OR  bs.SupplierID IN (SELECT * FROM @SupplierIDs))
					 AND bs.CustomerID=@CustomerID
					 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			         AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
				GROUP BY Z.Name, bs.SupplierID,bs.Cost_RateType--,bs.Cost_Rate/ISNULL(ERC.Rate, 1)
				ORDER BY Z.Name ASC
		END
		ELSE
			IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
			BEGIN 
				SELECT 
					Z.Name AS Zone,
					bs.SupplierID AS SupplierID,
					SUM(bs.NumberOfCalls) AS Calls,
					AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
					--bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
					bs.Cost_RateType AS RateType,
					SUM(bs.CostDuration/60.0) AS DurationInSeconds,
					--(bs.Cost_Rate/ISNULL(ERC.Rate, 1))*(SUM(bs.CostDuration/60.0)) AS Net
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
					SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
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
				GROUP BY Z.Name, bs.SupplierID,bs.Cost_RateType--,bs.Cost_Rate/ISNULL(ERC.Rate, 1)
				ORDER BY Z.Name ASC 		
			END

			SELECT ((SUM((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1)))*@NumberOfDays2) AS Services
			FROM CarrierAccount ca 
			JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
			LEFT JOIN @ExchangeRates ER ON ER.CurrencyIn = cp.CurrencyID AND ER.CurrencyOut = @CurrencyID AND ER.Date = @FromDate
	END
	ELSE 
	BEGIN 
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT 
				Z.Name AS Zone,
				bs.SupplierID AS SupplierID,
				SUM(bs.NumberOfCalls) AS Calls,
				--bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
				AVG(ISNULL(bs.Sale_Rate/ISNULL(ERS.Rate, 1),0)) AS Rate,
				bs.Sale_RateType AS RateType,
				SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
				--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
				SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
				SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
				SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
			LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
			WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
			AND(@CustomerAmuID IS NULL OR  bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR  bs.SupplierID IN (SELECT * FROM @SupplierIDs))
			AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
		    AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
			GROUP BY Z.Name, bs.SupplierID,bs.Sale_RateType--,bs.Sale_Rate/ISNULL(ERS.Rate, 1)
			ORDER BY Z.Name ASC
		END
		ELSE 
			IF(@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN
				SELECT 
					Z.Name AS Zone,
					bs.SupplierID AS SupplierID,
					SUM(bs.NumberOfCalls) AS Calls,
					AVG(ISNULL(bs.Sale_Rate/ISNULL(ERS.Rate, 1),0)) AS Rate,
					--bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
					bs.Sale_RateType AS RateType,
					SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
					--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
					SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
					SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
					SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
				LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
				WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
					AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
					 AND bs.SupplierID=@SupplierID
					 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			         AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
				GROUP BY Z.Name, bs.SupplierID,bs.Sale_RateType--,bs.Sale_Rate/ISNULL(ERS.Rate, 1)
				ORDER BY Z.Name ASC
			END
			ELSE
				IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
				BEGIN
					SELECT 
						Z.Name AS Zone,
						bs.SupplierID AS SupplierID,
						SUM(bs.NumberOfCalls) AS Calls,
						AVG(ISNULL(bs.Sale_Rate/ISNULL(ERS.Rate, 1),0)) AS Rate,
						--bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
						bs.Sale_RateType AS RateType,
						SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
						--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
						SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
						SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
						SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
						SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
					LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
					WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
						 AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
						 AND bs.CustomerID=@CustomerID
						 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			             AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
					GROUP BY Z.Name,bs.SupplierID,bs.Sale_RateType--,bs.Sale_Rate/ISNULL(ERS.Rate, 1)
					ORDER BY Z.Name ASC
				END
				ELSE
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
					BEGIN
						 SELECT 
							Z.Name AS Zone,
							bs.SupplierID AS SupplierID,
							SUM(bs.NumberOfCalls) AS Calls,
							AVG(ISNULL(bs.Sale_Rate/ISNULL(ERS.Rate, 1),0)) AS Rate,
							--bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
							bs.Sale_RateType AS RateType,
							SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
							--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
							SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
							SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
							SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
							SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
						LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
						LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
						WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
							 AND bs.CustomerID=@CustomerID
							 AND bs.SupplierID=@SupplierID
							 AND (@SupplierGroup IS NULL OR ','+@SupplierGroup+',' LIKE '%,'+bs.SupplierID+',%')
			                 AND (@CustomerGroup IS NULL OR ','+@CustomerGroup+',' LIKE+'%,'+ bs.CustomerID+',%' ) 
						GROUP BY Z.Name,bs.SupplierID,bs.Sale_RateType--,bs.Sale_Rate/ISNULL(ERS.Rate, 1)
						ORDER BY Z.Name ASC
					END
    END 
    END
	RETURN