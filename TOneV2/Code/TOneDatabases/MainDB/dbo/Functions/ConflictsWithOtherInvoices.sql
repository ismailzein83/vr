CREATE   FUNCTION [dbo].[ConflictsWithOtherInvoices]
(
	@InvoiceID int,
	@CustomerID varchar(10),
	@SupplierID varchar(10),
	@BeginDate smalldatetime,
	@EndDate smalldatetime
)
RETURNS char(1)
AS
BEGIN
	SET @InvoiceID = ISNULL(@InvoiceID, 0)
	RETURN 
		CASE 
			WHEN EXISTS (
					SELECT * FROM Billing_Invoice BI 
						WHERE BI.InvoiceID <> @InvoiceID 
							AND CustomerID = @CustomerID
							AND SupplierID = @SupplierID
							AND (
									BI.BeginDate BETWEEN @BeginDate AND EndDate 
									OR 
									BI.EndDate BETWEEN @BeginDate AND EndDate 
								)
							)
				THEN 'Y'
			ELSE 'N'
		END
END