
CREATE PROCEDURE [dbo].[sp_TrafficStats_ProfitDailyReport]
	@FromDate DATETIME,
	@ToDate   DATETIME, 
	@OurZoneID 	  INT = NULL,
	@SupplierID   varchar(5),
	@CustomerID   varchar(5),
	@PageIndex INT = 1,
	@PageSize INT = 10,
	@RecordCount INT OUTPUT
WITH RECOMPILE
AS
BEGIN

SET @ToDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDate)))
	
DECLARE @Traffic table(
        [date] DateTime,
        ZoneID int,
		SupplierID Varchar(5),
		CustomerID Varchar(5),
		Attempts INT,
		ASR NUMERIC(19,6),
		ACD NUMERIC(19,6),
		AveragePDD NUMERIC(19,6),
		DurationsInMinutes NUMERIC(19,6),
		SuccessfulAttempts BIGINT
                )
DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
insert into @Traffic(
           [Date] ,
           ZoneID,
           SupplierID,
           CustomerID,
           Attempts,
           ASR,
           ACD,
           AveragePDD,
           DurationsInMinutes,
           SuccessfulAttempts
                          )
  Select  dbo.dateof(TS.FirstCDRAttempt),
          ISNULL(TS.OurZoneID, ''), 
          ISNULL(TS.SupplierID, ''), 
		  ISNULL(TS.CustomerID, ''),
     	  Sum(TS.Attempts) as Attempts, 
		  Sum(TS.SuccessfulAttempts)*100.0 / Sum(TS.Attempts) as ASR, 
		  case when Sum(TS.SuccessfulAttempts) > 0 then Sum(TS.DurationsInSeconds)/(60.0*Sum(TS.SuccessfulAttempts)) ELSE 0 end as ACD, 
		  Avg(TS.PDDinSeconds) as AveragePDD,
		  Sum(TS.DurationsInSeconds/60.) as DurationsInMinutes,
		  Sum(TS.SuccessfulAttempts)AS SuccessfulAttempts
		  from TrafficStats TS WITH(NOLOCK)
		  --,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier),INDEX(IX_TrafficStats_Customer))
		     
		   WHERE (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID) 
		   and (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
		   and(@CustomerID IS NULL OR TS.CustomerID = @CustomerID)
		   and ( TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=  @ToDate )
		   AND ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		   Group by 
		   dbo.dateof(TS.FirstCDRAttempt),
		   ISNULL(TS.SupplierID, ''), 
		   ISNULL(TS.CustomerID, ''),
		   ISNULL(TS.OurZoneID, '')
    Declare @Billing table(
        [date] datetime,
        ZoneID int,
		SupplierID Varchar(5),
		CustomerID Varchar(5),
        Sale_Rate float,	
		Cost_Rate float
		
                          )
    insert into @Billing
    ( 
        [date],
        ZoneID,
		SupplierID,
		CustomerID,
        Sale_Rate, 
		Cost_Rate
		
    )
    select 
           dbo.dateof(BS.CallDate),
           BS.SaleZoneID,
		   BS.SupplierID,
		   BS.CustomerID,
           AVG(BS.Sale_Rate),
           AVG(BS.Cost_Rate)
		   from Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
		   LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
           LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate 
       	   where (@OurZoneID IS NULL OR BS.SaleZoneID = @OurZoneID) 
		   and (@SupplierID IS NULL OR BS.SupplierID = @SupplierID) 
		   and(@CustomerID IS NULL OR BS.CustomerID = @CustomerID)
		   and ( BS.CallDate >= @FromDate AND BS.CallDate <=  @ToDate )
           group by      
                        dbo.dateof(BS.CallDate),
                        BS.SaleZoneID,
		                BS.SupplierID,
		                BS.CustomerID
		                
	
	SELECT  
			T.[Date] ,
			ROW_NUMBER() OVER ( ORDER BY  T.[Date],T.Attempts DESC) AS rowNumber,
            T.ZoneID,
            T.SupplierID,
            T.CustomerID,
            T.Attempts,
            T.ASR,
            T.ACD,
            T.AveragePDD,
            T.DurationsInMinutes,
            T.SuccessfulAttempts,
            B.Sale_Rate, 
		    B.Cost_Rate
		    INTO #RESULT
            FROM @Traffic T left join @Billing B   on (T.ZoneID IS NULL OR B.ZoneID = T.ZoneID) 
		   and (T.SupplierID IS NULL OR B.SupplierID = T.SupplierID) 
		   and( T.CustomerID IS NULL OR B.CustomerID =  T.CustomerID)
		   and (B.Date = T.[date])
		   order by T.[Date],T.Attempts DESC
		   SELECT @RecordCount = COUNT(*)
	FROM #RESULT
	
	SELECT * FROM #RESULT
	WHERE RowNumber BETWEEN (@PageIndex - 1) * @PageSize + 1 AND (((@PageIndex -1) * @PageSize + 1) + @PageSize) -1
	
	DROP TABLE #RESULT
    -- Insert statements for procedure here

END