-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Billing].[sp_BillingInvoiceDetails_CreateTempByInvoiceID]
	@TempTableName VARCHAR(200),
	@InvoiceID INT
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SELECT
			ID AS DetailID,
			FromDate,
			TillDate,
			Destination,
			NumberOfCalls,
			Duration,
			Rate,
			RateType,
			Amount,
			CurrencyID
		
		INTO #RESULT
		FROM dbo.Billing_Invoice_Details
		
		WHERE InvoiceID = @InvoiceID
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END