
-- 
CREATE PROCEDURE [dbo].[EA_PospaidCarrierTotal]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y',
	@CarrierAccountID varchar(5) = NULL,
	@CarrierProfileID int
AS

	DECLARE @PostpaidType int
	SELECT @PostpaidType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.PaymentType' 
			AND [Name] = 'Postpaid'

	DECLARE @PostPaidStats TABLE
	(
		CarrierID varchar(10),
		ProfileID int,
		[Type] varchar(10) NOT NULL,
		CarrierName varchar(255),
		CurrencyID varchar(3),
		LastPaid smalldatetime NULL,
		UnpaidInvoicesNumber int NULL,
		UnpaidInvoicesAmount numeric(13,5) NULL,
		CreditLimit numeric(13,5) NULL,
		InvoiceDelayDays int NULL,
		LastDueDate smalldatetime NULL,
		Sale numeric(13,5) NULL,
		Purchase numeric(13,5) NULL,
		Net numeric(13,5) NULL,
        Tolerance numeric(13,5) NULL
	)
	
	 -- Customer
	INSERT INTO @PostPaidStats (
		CarrierID,
		ProfileID,
		[Type],
		CarrierName,
		CurrencyID,
		LastPaid,
		UnpaidInvoicesNumber, 
		UnpaidInvoicesAmount, 
		CreditLimit, 
		InvoiceDelayDays,
		LastDueDate,
		Sale,
		Purchase
	)
	SELECT 
		Ca.[CarrierAccountID] AS CarrierID,
		Ca.ProfileID,
		'Customer',
		Cp.Name,	
		Cp.CurrencyID,
		
		(SELECT TOP 1 PaidDate FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'Y' ORDER BY bi.PaidDate DESC) 
			AS [LastPaid],
		
		(SELECT Count(*) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesNumber],
		
		(SELECT Sum(bi.Amount) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesAmount],
		
		ca.CustomerCreditLimit,
		
		(SELECT TOP 1 datediff(dd, getdate(), bi.DueDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [Invoice Delay],
		
		(SELECT TOP 1 bi.DueDate FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [LastDueDate],

		(SELECT Sum(bs.Sale_Nets) FROM Billing_Stats bs WHERE bs.CustomerID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID),'1990-01-01')
			) AS [Sale], 
		
		(SELECT Sum(bs.Cost_Nets) FROM Billing_Stats bs WHERE bs.SupplierID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID),'1990-01-01')
			) AS [Purchase] 
	
	FROM CarrierAccount ca, CarrierProfile cp  
		WHERE 
				(ca.CustomerPaymentType = @PostpaidType OR cp.CustomerPaymentType = @PostpaidType)				 
			AND ca.ProfileID = cp.ProfileID
			AND (ca.CustomerCreditLimit > 0 OR cp.CustomerCreditLimit > 0)
			AND (ca.CarrierAccountID = @CarrierAccountID OR ca.ProfileID = @CarrierProfileID)
	ORDER BY cp.Name
	
   -- supplier
      INSERT INTO @PostPaidStats (
		CarrierID, 
		ProfileID,
		[Type],
		CarrierName,
		CurrencyID,
		LastPaid,
		UnpaidInvoicesNumber, 
		UnpaidInvoicesAmount, 
		CreditLimit, 
		InvoiceDelayDays,
		LastDueDate,
		Sale,
		Purchase
	)
	SELECT 
		Ca.[CarrierAccountID] AS CarrierID,
		Ca.ProfileID,
		'Supplier',
		Cp.Name,	
		Cp.CurrencyID,
		
		(SELECT TOP 1 PaidDate FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'Y' ORDER BY bi.PaidDate DESC) 
			AS [LastPaid],
		
		(SELECT Count(*) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesNumber],
		
		(SELECT Sum(bi.Amount) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesAmount],
		
		ca.SupplierCreditLimit,
		
		(SELECT TOP 1 datediff(dd, getdate(), bi.DueDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [Invoice Delay],
		
		(SELECT TOP 1 bi.DueDate FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [LastDueDate],

		(SELECT Sum(bs.Sale_Nets) FROM Billing_Stats bs WHERE bs.CustomerID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID),'1990-01-01')
			) AS [Sale], 
		
		(SELECT Sum(bs.Cost_Nets) FROM Billing_Stats bs WHERE bs.SupplierID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID),'1990-01-01')
			) AS [Purchase] 
	
	FROM CarrierAccount ca, CarrierProfile cp  
		WHERE 
				(ca.SupplierPaymentType = @PostpaidType OR cp.SupplierPaymentType = @PostpaidType)
			AND ca.ProfileID = cp.ProfileID
			AND (ca.SupplierCreditLimit > 0 OR cp.SupplierCreditLimit > 0 )
			AND (ca.CarrierAccountID = @CarrierAccountID OR ca.ProfileID = @CarrierProfileID)
	ORDER BY cp.Name
	
	UPDATE @PostPaidStats SET Net = ISNULL(Sale,0) - ISNULL(Purchase,0)
	UPDATE @PostPaidStats SET Tolerance = (case WHEN CreditLimit = 0 then 0 else (ISNULL(Net,0) * 100) / CreditLimit end )

   IF @ShowCustomerTotal = 'Y'   
		SELECT * FROM @PostPaidStats WHERE Type='Customer'
   IF @ShowSupplierTotal = 'Y'
		SELECT * FROM @PostPaidStats WHERE Type='Supplier'