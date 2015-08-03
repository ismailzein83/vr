 

CREATE PROCEDURE [Analytics].[SP_BillingStats_GetTrafficVolumes](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@OurZoneID INT,
	@Attempts INT,
	@Period varchar(7)
)
with Recompile
AS 
	
   IF(@Period = 'None')
   BEGIN
   	   SELECT 
			  SUM(BS.NumberOfCalls) AS Attempts,
			  SUM(BS.SaleDuration)/60.0 AS Duration    
	   FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
	   WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
			AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
			AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
			AND (@OurZoneID = 0 OR BS.SaleZoneID=@OurZoneID)		
			AND  BS.NumberOfCalls > @Attempts
   END
	ELSE	
		IF(@Period = 'Daily')
		BEGIN
		   SELECT 
				  cast(BS.CallDate AS varchar(12)) AS CallDate,
				  SUM(BS.NumberOfCalls) AS Attempts,
				  SUM(BS.SaleDuration)/60.0 AS Duration    
		   FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		   WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
 				AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
				AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
				AND (@OurZoneID = 0 OR BS.SaleZoneID=@OurZoneID)		
				AND  BS.NumberOfCalls > @Attempts
		   GROUP BY BS.calldate 
		   ORDER BY BS.Calldate ASC 	 
		END
		ELSE
			IF(@Period = 'Weekly')
			BEGIN
				SELECT 
					datepart(week,BS.CallDate) AS CallWeek,
					datepart(year,bs.CallDate) AS CallYear,
					SUM(BS.NumberOfCalls) AS Attempts,
					SUM(BS.SaleDuration)/60.0 AS Duration    
				FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
				WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
					AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
					AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
					AND (@OurZoneID = 0 OR BS.SaleZoneID=@OurZoneID)		
					AND  BS.NumberOfCalls > @Attempts
				GROUP BY DATEPART(week,BS.calldate),DATEPART(year,BS.calldate)
				ORDER BY DATEPART(year,BS.calldate),DATEPART(week,BS.calldate) ASC
			END
			ELSE
				IF(@Period = 'Monthly')
				BEGIN
					SELECT 
						datepart(month,BS.CallDate) AS CallMonth,
						datepart(year,bs.CallDate) AS CallYear,
						SUM(BS.NumberOfCalls) AS Attempts,
						SUM(BS.SaleDuration)/60.0 AS Duration    
					FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
					WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
						AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
						AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
						AND (@OurZoneID = 0 OR BS.SaleZoneID=@OurZoneID)		
						AND  BS.NumberOfCalls > @Attempts
					GROUP BY DATEPART(month,BS.calldate),DATEPART(year,BS.calldate)
					ORDER BY DATEPART(year,BS.calldate),DATEPART(month,BS.calldate) ASC 
				END
RETURN