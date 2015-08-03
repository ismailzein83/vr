CREATE PROCEDURE [dbo].[rpt_CDRDetails](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@FromDuration float,
	@ToDuration float
	)
WITH RECOMPILE
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

 SELECT TOP 1000 * FROM Billing_CDR_Invalid bci WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt),INDEX(IX_Billing_CDR_Invalid_Supplier),INDEX(IX_Billing_CDR_Invalid_Customer))  
     WHERE 1=1
        AND (Attempt Between @FromDate And @ToDate)
		AND (@CustomerID IS NULL OR CustomerID = @CustomerID)
		AND (@SupplierID IS NULL OR SupplierID = @SupplierID)
		AND (@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND (@ToDuration IS NULL OR DurationInSeconds <= @ToDuration)	           
RETURN