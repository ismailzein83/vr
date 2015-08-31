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
		WITH CarriersCTE AS
		(
			SELECT ca.CarrierAccountID, cp.Name AS SupplierName, ca.NameSuffix AS SupplierNameSuffix
			FROM CarrierAccount ca INNER JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
			WHERE ca.AccountType IN (1, 2)
		)
		
		SELECT
			bi.InvoiceID,
			bi.SupplierID,
			cte.SupplierName,
			cte.SupplierNameSuffix,
			bi.UserID,
			u.Name AS UserName,
			bi.SerialNumber,
			bi.BeginDate,
			bi.EndDate,
			bi.InvoiceNotes AS TimeZone,
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
		INNER JOIN CarriersCTE cte ON cte.CarrierAccountID = bi.SupplierID
		LEFT JOIN dbo.[User] u ON bi.UserID = u.ID
		
		WHERE
			--CustomerID = 'SYS'
			SupplierID = @SelectedSupplierID
			AND ((bi.BeginDate BETWEEN @from AND @to) OR (bi.EndDate BETWEEN @from AND @to))
			--AND (BeginDate >= @from and EndDate <= @to) This makes more sense to me, but I'm going with the implementation of the old sp
		
		ORDER BY IssueDate DESC
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END