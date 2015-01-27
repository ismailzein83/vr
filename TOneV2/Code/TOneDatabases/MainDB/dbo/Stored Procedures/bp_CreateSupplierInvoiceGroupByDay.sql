
CREATE PROCEDURE [dbo].[bp_CreateSupplierInvoiceGroupByDay]
@SupplierID varchar(5),
@FromDate Datetime,
@ToDate   Datetime,
@GMTShifttime float
AS

BEGIN
	
SET @FromDate = DATEADD(day,0,datediff(day,0, @FromDate))
SET @ToDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDate)))

	DECLARE @Results TABLE ( 
		                   [Day]		Datetime,
		                   [Currency] VARCHAR(3),
						   Duration 	float,
						   Amount		float
							) 

	INSERT INTO @Results	(
							[Day],
							[Currency],
							Duration,
							Amount
							)	
    
	SELECT CONVERT(varchar(10),dateadd(mi,-@GMTShifttime,BM.Attempt),121) as [Day],
	       bc.CurrencyID,
           SUM(BC.DurationInSeconds)/60.0 AS Duration,
           ISNULL(SUM(BC.NET),0) AS Amount
    FROM   Billing_CDR_Main BM WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt,IX_Billing_CDR_Main_Supplier))
    left join Billing_CDR_Cost BC WITH(nolock)
      ON   BM.ID = BC.ID
    WHERE  BM.SupplierID = @SupplierID
      AND  BM.Attempt >= @FromDate
      AND  BM.Attempt <=  @ToDate
    GROUP BY
           CONVERT(varchar(10),dateadd(mi,-@GMTShifttime,BM.Attempt),121),
           bc.CurrencyID
    ORDER BY
           CONVERT(varchar(10),dateadd(mi,-@GMTShifttime,BM.Attempt),121)
	
    SELECT		  CASE
					WHEN @GMTShifttime <= 0  THEN CONVERT(varchar(10),([Day]),121)
					WHEN @GMTShifttime > 0  THEN CONVERT(varchar(10),[Day]+1,121)
				  END AS [Day],
				         Currency,
						 Duration,
						 Amount
    FROM			@Results 
	ORDER BY		[day]

END