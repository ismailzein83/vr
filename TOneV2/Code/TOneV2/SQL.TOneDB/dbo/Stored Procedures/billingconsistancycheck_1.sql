CREATE PROCEDURE [dbo].[billingconsistancycheck]
 @fromdate VARCHAR(20),
 @tilldate VARCHAR(20)

AS
BEGIN

DECLARE @CDR VARCHAR(50)
DECLARE @main VARCHAR(50)
DECLARE @sale VARCHAR(50)

SET @fromdate='2011-08-15 00:00:00'
SET @tilldate='2011-08-15 23:59:59'

SELECT @CDR= SUM(DurationInSeconds)/60
  FROM cdr WITH(NOLOCK,INDEX=IX_CDR_AttemptDateTime) 
WHERE AttemptDateTime BETWEEN @fromdate AND @tilldate

SELECT @main =SUM(DurationInSeconds)/60
  FROM billing_CDR_main WITH(NOLOCK,INDEX=IX_Billing_CDR_Main_Attempt) 
WHERE attempt BETWEEN @fromdate AND @tilldate

SELECT @sale =SUM(bcs.DurationInSeconds)/60
FROM Billing_CDR_Sale bcs WITH(NOLOCK,INDEX=PK_Billing_CDR_Sale)
WHERE id IN ( SELECT id 
                 FROM billing_CDR_main WITH(NOLOCK,INDEX=IX_Billing_CDR_Main_Attempt) 
WHERE attempt BETWEEN @fromdate AND @tilldate)

PRINT ' Viewing Data from the period from' + @fromdate + 'till' + @tilldate
PRINT 'duration of CDRs for the period =' + @CDR
PRINT 'duration of the priced CDRs for the period =' + @main
PRINT 'duration of Sale minutes for the period =' + @sale


END