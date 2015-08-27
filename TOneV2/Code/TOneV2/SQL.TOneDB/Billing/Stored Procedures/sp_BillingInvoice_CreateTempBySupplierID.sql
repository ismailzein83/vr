-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Billing].[sp_BillingInvoice_CreateTempBySupplierID]
	@TempTableName VARCHAR(200),
	@SelectedSupplierID VARCHAR(5),
	@From DATETIME,
	@To DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SELECT
			bi.InvoiceID,
			bi.SupplierID,
			bi.UserID,
			u.Name AS UserName,
			bi.SerialNumber,
			bi.BeginDate,
			bi.EndDate,
			bi.IssueDate,
			bi.DueDate,
			bi.CreationDate,
			bi.NumberOfCalls,
			bi.Duration,
			bi.Amount,
			bi.CurrencyID,
			bi.IsLocked,
			bi.IsPaid,
			bi.InvoiceNotes
		
		INTO #RESULT
		FROM dbo.Billing_Invoice bi
		LEFT JOIN dbo.[User] u ON bi.UserID = u.ID
		
		WHERE --CustomerID = 'sys'
			SupplierID = @SelectedSupplierID
			--and ((bi.BeginDate between @from and @to) or (bi.EndDate between @from and @to))
			AND (BeginDate >= @from and EndDate <= @to)
		
		ORDER BY IssueDate DESC
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END