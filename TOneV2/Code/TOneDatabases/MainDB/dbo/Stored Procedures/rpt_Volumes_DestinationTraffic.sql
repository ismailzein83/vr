



CREATE  PROCEDURE dbo.rpt_Volumes_DestinationTraffic(
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@OurZoneID INT,
	@TopValues INT,
	@Attempts INT,
	@Period varchar(7)
)
with Recompile
AS 
	SET @FromDate= CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
 IF @OurZoneID=0
 SET @OurZoneID = null 
 SET ROWCOUNT @TopValues
 
 IF(@Period = 'None')
 BEGIN
   SELECT 
          bs.SaleZoneID AS SaleZoneID,
          SUM(BS.NumberOfCalls) AS Attempts,
          SUM(BS.SaleDuration)/60.0 AS Duration    
   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
     	AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
	    AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
	    AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
        AND  BS.NumberOfCalls > @Attempts
   GROUP BY bs.SaleZoneID
   ORDER BY Duration DESC	
 END
 ELSE
 	IF(@Period = 'Daily')
	BEGIN
	   SELECT 
			  bs.CallDate AS CallDate,
			  bs.SaleZoneID AS SaleZoneID,
			  SUM(BS.NumberOfCalls) AS Attempts,
			  SUM(BS.SaleDuration)/60.0 AS Duration    
	   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
	   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
 			AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
			AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
			AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
			AND  BS.NumberOfCalls > @Attempts
	   GROUP BY bs.CallDate, bs.SaleZoneID
	   ORDER BY Duration DESC	
	END
	ELSE
	   IF(@Period = 'Weekly')
       BEGIN
		   SELECT 
				  datepart(week,BS.CallDate) AS CallWeek,
				  datepart(year,bs.CallDate) AS CallYear,
				  bs.SaleZoneID AS SaleZoneID,
				  SUM(BS.NumberOfCalls) AS Attempts,
				  SUM(BS.SaleDuration)/60.0 AS Duration    
		   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
				AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
				AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
				AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
				AND  BS.NumberOfCalls > @Attempts
		   GROUP BY DATEPART(week,BS.calldate),DATEPART(year,BS.calldate), bs.SaleZoneID
		   ORDER BY Duration DESC	
	   END
		ELSE
		  IF(@Period = 'Monthly')
	      BEGIN
		   SELECT 
			      datepart(month,BS.CallDate) AS CallMonth,
			      datepart(year,bs.CallDate) AS CallYear,
				  bs.SaleZoneID AS SaleZoneID,
				  SUM(BS.NumberOfCalls) AS Attempts,
				  SUM(BS.SaleDuration)/60.0 AS Duration    
		   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
				AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
				AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
				AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
				AND  BS.NumberOfCalls > @Attempts
		   GROUP BY DATEPART(month,BS.calldate),DATEPART(year,BS.calldate), bs.SaleZoneID
		   ORDER BY Duration DESC	
	      END
 SET ROWCOUNT 0
RETURN