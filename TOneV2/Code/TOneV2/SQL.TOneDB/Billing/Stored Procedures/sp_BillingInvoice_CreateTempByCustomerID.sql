

CREATE PROCEDURE [Billing].[sp_BillingInvoice_CreateTempByCustomerID]
	@TempTableName VARCHAR(200),
	@SelectedCustomerID VARCHAR(5),
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	
	SELECT
        B.InvoiceId AS InvoiceId,
        B.CustomerId AS CustomerId,
        CPC.Name AS CustomerName,
        C.NameSuffix AS CustomerNameSuffix,
        B.SupplierId AS SupplierId,
        CPS.Name AS SupplierName,
        S.NameSuffix AS SupplierNameSuffix,
        B.UserId AS UserId,
        U.Name AS UserName,
        B.SerialNumber AS SerialNumber,
        B.BeginDate AS BeginDate,
        B.EndDate AS EndDate,
        t.DisplayName AS TimeZone,
        B.IssueDate AS IssueDate,
        B.DueDate AS DueDate,
        B.CreationDate AS CreationDate,
        B.PaidDate AS PaidDate,
        B.NumberOfCalls AS NumberOfCalls,
        B.Duration AS Duration,
        B.Amount AS Amount,
        B.CurrencyId AS CurrencyId,
        B.IsLocked AS IsLocked,
        B.IsPaid AS IsPaid,
        B.IsAutomatic AS IsAutomatic,
        B.IsSent AS IsSent,
        B.InvoiceNotes AS InvoiceNotes
    INTO #RESULT
    FROM Billing_Invoice B 
    JOIN CarrierAccount C ON C.CarrierAccountId = B.CustomerID 
    JOIN CarrierAccount S ON S.CarrierAccountID = B.SupplierID
    JOIN CarrierProfile CPC ON CPC.ProfileId = C.ProfileId
    JOIN CarrierProfile CPS ON CPS.ProfileId = C.ProfileId
    JOIN CustomTimeZoneinfo t ON t.BaseUtcOffset = C.GMTTime
    JOIN [User] U ON U.[Id] = B.UserId
    WHERE B.CustomerId = @SelectedCustomerID  
    AND (B.BeginDate BETWEEN @FromDate AND @ToDate OR B.EndDate BETWEEN @FromDate AND @ToDate ) 
    ORDER BY B.IssueDate DESC
    
    DECLARE @sql VARCHAR(1000)
	SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
	EXEC(@sql)
END