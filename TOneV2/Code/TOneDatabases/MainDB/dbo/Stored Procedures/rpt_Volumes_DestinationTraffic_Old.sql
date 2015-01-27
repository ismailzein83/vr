
CREATE  PROCEDURE dbo.rpt_Volumes_DestinationTraffic_Old(
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@OurZoneID INT,
	@TopValues INT,
	@Attempts INT
)
with Recompile
AS 
	SET @FromDate=     CAST(
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
 SET ROWCOUNT 0
 
RETURN